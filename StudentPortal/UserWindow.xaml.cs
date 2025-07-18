using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentPortal
{
    public partial class UserWindow : Window
    {
        private readonly AppDbContext _db;
        private ObservableCollection<Student> _students = new ObservableCollection<Student>();

        public UserWindow(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            StudentsListBox.ItemsSource = _students;
            GroupComboBox.ItemsSource = _db.Groups.ToList();
            CourseComboBox.ItemsSource = _db.Courses.ToList();
            LoadStudents();
        }

        private void LoadStudents()
        {
            _students.Clear();
            IQueryable<Student> query = _db.Students;

            if (GroupComboBox.SelectedItem is Group selectedGroup)
            {
                query = query.Where(s => s.GroupId == selectedGroup.GroupId);
            }

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                query = query.Where(s => s.Familiya.ToLower().Contains(searchText));
            }

            foreach (var student in query.ToList())
            {
                _students.Add(student);
            }

            CourseComboBox.IsEnabled = GroupComboBox.SelectedItem != null;
            if (GroupComboBox.SelectedItem is Group selectedGroupForCourse)
            {
                CourseComboBox.SelectedItem = selectedGroupForCourse.Course;
            }
        }

        private void LoadGrades(Student student)
        {
            var grades = _db.GradeStudents
                .Where(gs => gs.StudentId == student.StudentId)
                .Select(gs => gs.Grade.GradeValue)
                .ToList();
            GradesListBox.ItemsSource = grades;
        }

        private void CalculateAverageGrade(Student student)
        {
            var grades = _db.GradeStudents
                .Where(gs => gs.StudentId == student.StudentId)
                .Select(gs => gs.Grade.GradeValue);

            if (grades.Any())
            {
                double averageGrade = grades.Average();
                AverageGradeTextBlock.Text = averageGrade.ToString("N2");
            }
            else
            {
                AverageGradeTextBlock.Text = "0.00";
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadStudents();
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStudents();
            CourseComboBox.IsEnabled = GroupComboBox.SelectedItem != null;
            if (GroupComboBox.SelectedItem is Group selectedGroupForCourse)
            {
                CourseComboBox.SelectedItem = selectedGroupForCourse.Course;
            }
        }

        private void StudentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsListBox.SelectedItem is Student selectedStudent)
            {
                LoadGrades(selectedStudent);
                CalculateAverageGrade(selectedStudent);
            }
            else
            {
                GradesListBox.ItemsSource = null;
                AverageGradeTextBlock.Text = "-";
            }
        }
    }
}