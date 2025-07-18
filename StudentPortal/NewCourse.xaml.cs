using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using StudentPortal.Data;

namespace StudentPortal
{
    public partial class NewCourse : Page
    {
        private readonly AppDbContext _db;
        private StudentPortal.Course _currentCourse; // Явно указано StudentPortal.Course

        public NewCourse(AppDbContext db, StudentPortal.Course course)
        {
            InitializeComponent();
            _db = db;
            _currentCourse = course ?? new StudentPortal.Course(); // Явно указано StudentPortal.Course
            DataContext = _currentCourse;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (string.IsNullOrWhiteSpace(_currentCourse.CourseName))
                    errors.AppendLine("Введите название курса!");

                if (_currentCourse.Duration <= 0)
                    errors.AppendLine("Введите продолжительность курса!");

                if (string.IsNullOrWhiteSpace(_currentCourse.Description))
                    errors.AppendLine("Введите описание!");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }

                if (_currentCourse.CourseId == 0)
                {
                    _currentCourse.CourseName = name.Text;
                    _currentCourse.Duration = int.Parse(prod.Text);
                    _currentCourse.Description = opisanie.Text;
                    _db.Courses.Add(_currentCourse);
                }
                else
                {
                    _currentCourse.CourseName = name.Text;
                    _currentCourse.Duration = int.Parse(prod.Text);
                    _currentCourse.Description = opisanie.Text;
                    _db.Entry(_currentCourse).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                _db.SaveChanges();
                MessageBox.Show("Запись сохранена!");
                Manager.MainFrame.Navigate(new CoursePage(_db)); // Навигация на CoursePage
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}