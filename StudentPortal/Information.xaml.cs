using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ClosedXML.Excel;
using StudentPortal.Data;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Drawing;
using System.IO;

namespace StudentPortal
{
    public partial class Information : Page
    {
        private readonly AppDbContext _db;
        private ObservableCollection<Student> _students;
        private List<Student> _filteredStudents; // Для хранения отфильтрованного списка
        private const int PageSize = 4; // Количество карточек на странице (2 ряда по 2 карточки)
        private int _currentPage = 1;
        private int _totalPages = 1;

        public Information(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadStudents();
        }

        public void LoadStudents()
        {
            try
            {
                _students = new ObservableCollection<Student>(_db.Students.Include(s => s.Group).ToList());
                _filteredStudents = _students.ToList();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки студентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            _totalPages = (int)Math.Ceiling((double)_filteredStudents.Count / PageSize);
            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));

            // Отображаем только элементы текущей страницы
            var studentsToShow = _filteredStudents
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            StudentsList.ItemsSource = new ObservableCollection<Student>(studentsToShow);

            // Обновляем информацию о странице
            PageInfoText.Text = $"Страница {_currentPage} из {_totalPages}";
            PrevPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _totalPages;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredStudents = _students.ToList();
            }
            else
            {
                _filteredStudents = _students
                    .Where(s =>
                        s.Imya.ToLower().Contains(searchText) ||
                        s.Familiya.ToLower().Contains(searchText) ||
                        s.Otchestvo.ToLower().Contains(searchText))
                    .ToList();
            }

            _currentPage = 1; // Сбрасываем страницу на первую при поиске
            UpdatePagination();
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagination();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdatePagination();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Student selected)
            {
                Manager.MainFrame.Navigate(new NewInformation(_db, selected, this));
            }
            else
            {
                MessageBox.Show("Выберите студента для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnadd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new NewInformation(_db, null, this));
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_students == null || _students.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта в Excel.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
                Title = "Выберите место для сохранения файла",
                FileName = "StudentsExport.xlsx"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Студенты");

                    worksheet.Cell(1, 1).Value = "Имя";
                    worksheet.Cell(1, 2).Value = "Фамилия";
                    worksheet.Cell(1, 3).Value = "Отчество";
                    worksheet.Cell(1, 4).Value = "Дата рождения";
                    worksheet.Cell(1, 5).Value = "Email";
                    worksheet.Cell(1, 6).Value = "Телефон";

                    for (int i = 0; i < _students.Count; i++)
                    {
                        var student = _students[i];
                        worksheet.Cell(i + 2, 1).Value = student.Imya;
                        worksheet.Cell(i + 2, 2).Value = student.Familiya;
                        worksheet.Cell(i + 2, 3).Value = student.Otchestvo;
                        worksheet.Cell(i + 2, 4).Value = student.DateOfBirth?.ToString("d");
                        worksheet.Cell(i + 2, 5).Value = student.Email;
                        worksheet.Cell(i + 2, 6).Value = student.PhoneNumber;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(saveFileDialog.FileName);
                }

                MessageBox.Show("Данные успешно экспортированы в Excel!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
 
        private void btndel_Click(object sender, RoutedEventArgs e)
        {
            Student studentToDelete = StudentsList.SelectedItem as Student;

            if (studentToDelete == null)
            {
                MessageBox.Show("Выберите студента для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите удалить {studentToDelete.Familiya} {studentToDelete.Imya}?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Student studentToDeleteFromDb = _db.Students.Find(studentToDelete.StudentId);

                    if (studentToDeleteFromDb == null)
                    {
                        MessageBox.Show("Студент не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var gradeStudentsToDelete = _db.GradeStudents.Where(gs => gs.StudentId == studentToDeleteFromDb.StudentId).ToList();
                    foreach (var gradeStudent in gradeStudentsToDelete)
                    {
                        _db.GradeStudents.Remove(gradeStudent);
                    }

                    _db.Students.Remove(studentToDeleteFromDb);
                    _db.SaveChanges();
                    MessageBox.Show("Студент успешно удалён!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}\nInnerException: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}