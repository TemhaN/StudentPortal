using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StudentPortal.Data;

namespace StudentPortal
{
    public partial class CoursePage : Page
    {
        private readonly AppDbContext _db;
        private ObservableCollection<StudentPortal.Course> _courses;

        public CoursePage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            _courses = new ObservableCollection<StudentPortal.Course>(_db.Courses.ToList());
            DGridCourse.ItemsSource = _courses;
            LoadCourses();
        }

        private void LoadCourses()
        {
            _courses = new ObservableCollection<StudentPortal.Course>(_db.Courses.ToList());
            DGridCourse.ItemsSource = _courses;
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new NewCourse(_db, null));
        }

        private void btndel_Click(object sender, RoutedEventArgs e)
        {
            StudentPortal.Course courseToDelete = (StudentPortal.Course)DGridCourse.SelectedItem;

            if (courseToDelete == null)
            {
                MessageBox.Show("Пожалуйста, выберите курс для удаления.", "Внимание");
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить курс {courseToDelete.CourseName} {courseToDelete.Duration} {courseToDelete.Description}?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StudentPortal.Course courseToDeleteFromDb = _db.Courses.Find(courseToDelete.CourseId);

                    if (courseToDeleteFromDb == null)
                    {
                        MessageBox.Show("Курс не найден в базе данных.", "Ошибка");
                        return;
                    }

                    _db.Courses.Remove(courseToDeleteFromDb);
                    _db.SaveChanges();
                    MessageBox.Show("Курс успешно удален.", "Информация");
                    LoadCourses();
                }
                catch
                {
                    MessageBox.Show("На этом курсе есть группы. Его нельзя удалить.");
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is StudentPortal.Course selected)
            {
                Manager.MainFrame.Navigate(new NewCourse(_db, selected));
            }
            else
            {
                MessageBox.Show("Ошибка при получении данных.", "Внимание");
            }
        }
    }
}