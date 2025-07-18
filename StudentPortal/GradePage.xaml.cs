using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StudentPortal.Data;

namespace StudentPortal
{
    public partial class GradePage : Page
    {
        private readonly AppDbContext _db;
        private ObservableCollection<Student> _students = new ObservableCollection<Student>();
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();

        public GradePage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            GradeTextBox.ItemsSource = new[] { "2", "3", "4", "5" };
            LoadStudents();
            LoadSubjects();
        }

        private void LoadStudents()
        {
            try
            {
                var studentsWithGroups = _db.Students.Include(s => s.Group).ToList();
                _students.Clear();
                foreach (var student in studentsWithGroups)
                {
                    _students.Add(student);
                }
                StudentsListBox.ItemsSource = _students;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка с загрузкой студентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StudentsListBox.ItemsSource = null;
            }
        }

        private void LoadSubjects()
        {
            try
            {
                var subjects = _db.Subjects.ToList();
                _subjects.Clear();
                foreach (var subject in subjects)
                {
                    _subjects.Add(subject);
                }
                SubjectComboBox.ItemsSource = _subjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка с загрузкой предметов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string searchText = SearchTextBox.Text.ToLower();
                var filteredStudents = _db.Students
                    .Include(s => s.Group)
                    .Where(s => s.Familiya.ToLower().Contains(searchText))
                    .ToList();
                _students.Clear();
                foreach (var student in filteredStudents)
                {
                    _students.Add(student);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка с поиском: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StudentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка с выбором студента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGrades(Student student)
        {
            try
            {
                var grades = _db.GradeStudents
                    .Where(gs => gs.StudentId == student.StudentId)
                    .Include(gs => gs.Grade)
                    .ThenInclude(g => g.Subject)
                    .Select(gs => new
                    {
                        gs.Grade.GradeValue,
                        gs.Grade.GradeDate,
                        gs.Grade.Subject.SubjectName
                    })
                    .OrderByDescending(gs => gs.GradeDate) // Сортировка по дате (новые сверху)
                    .ToList();
                GradesListBox.ItemsSource = grades;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка с загрузкой оценок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateAverageGrade(Student student)
        {
            try
            {
                var grades = _db.GradeStudents
                    .Where(gs => gs.StudentId == student.StudentId)
                    .Select(gs => gs.Grade.GradeValue);
                AverageGradeTextBlock.Text = grades.Any() ? grades.Average().ToString("N2") : "0.00";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка с подсчётом среднего балла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddGradeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StudentsListBox.SelectedItem is Student selectedStudent && SubjectComboBox.SelectedItem is Subject selectedSubject)
                {
                    if (!int.TryParse(GradeTextBox.SelectedItem?.ToString(), out int newGradeValue) || newGradeValue < 2 || newGradeValue > 5)
                    {
                        MessageBox.Show("Выбери оценку от 2 до 5.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var grade = new Grade
                    {
                        GradeValue = newGradeValue,
                        SubjectId = selectedSubject.SubjectId,
                        GradeDate = DateTime.Now
                    };
                    _db.Grades.Add(grade);
                    _db.SaveChanges();

                    var newGradeStudent = new GradeStudent
                    {
                        StudentId = selectedStudent.StudentId,
                        GradeId = grade.GradeId
                    };
                    _db.GradeStudents.Add(newGradeStudent);
                    _db.SaveChanges();

                    LoadGrades(selectedStudent);
                    CalculateAverageGrade(selectedStudent);
                    GradeTextBox.SelectedItem = null;
                    MessageBox.Show("Оценка добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Выбери студента и предмет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не добавилось: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}