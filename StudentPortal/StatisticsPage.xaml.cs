using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using StudentPortal.Data;
using System.Windows;

namespace StudentPortal
{
    public partial class StatisticsPage : Page
    {
        private readonly AppDbContext _db;

        public StatisticsPage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                // Средние оценки по группам
                var groupAverages = _db.Groups
                    .Select(g => new KeyValuePair<string, double>(
                        g.GroupName,
                        _db.GradeStudents
                            .Where(gs => gs.Student.GroupId == g.GroupId)
                            .Select(gs => (double?)gs.Grade.GradeValue)
                            .Average() ?? 0))
                    .ToList();

                // Средние оценки по предметам
                var subjectAverages = _db.Subjects
                    .Select(s => new KeyValuePair<string, double>(
                        s.SubjectName,
                        _db.GradeStudents
                            .Where(gs => gs.Grade.SubjectId == s.SubjectId)
                            .Select(gs => (double?)gs.Grade.GradeValue)
                            .Average() ?? 0))
                    .ToList();

                var averages = new List<KeyValuePair<string, double>>();
                averages.AddRange(groupAverages);
                averages.AddRange(subjectAverages);
                AverageGradesListBox.ItemsSource = averages;

                // Топ студентов по среднему баллу
                var topStudents = _db.Students
                    .Select(s => new
                    {
                        FullName = s.Familiya + " " + s.Imya + " " + s.Otchestvo,
                        AverageGrade = _db.GradeStudents
                            .Where(gs => gs.StudentId == s.StudentId)
                            .Select(gs => (double?)gs.Grade.GradeValue)
                            .Average() ?? 0
                    })
                    .Where(s => s.AverageGrade > 0) // Исключаем студентов без оценок
                    .OrderByDescending(s => s.AverageGrade)
                    .Take(3) // Топ-3 студента
                    .ToList()
                    .Select(s => new { s.FullName, s.AverageGrade })
                    .ToList();

                TopStudentsListBox.ItemsSource = topStudents;

                // Распределение оценок
                var gradeCounts = _db.GradeStudents
                    .GroupBy(gs => gs.Grade.GradeValue)
                    .Select(g => new { Grade = g.Key, Count = g.Count() })
                    .OrderBy(g => g.Grade)
                    .ToList();

                var labels = new[] { "2", "3", "4", "5" };
                var values = new ChartValues<int> { 0, 0, 0, 0 };
                foreach (var gc in gradeCounts)
                {
                    if (gc.Grade >= 2 && gc.Grade <= 5)
                        values[gc.Grade - 2] = gc.Count;
                }

                if (GradeDistributionChart == null)
                {
                    MessageBox.Show("Ошибка: GradeDistributionChart не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (values == null)
                {
                    MessageBox.Show("Ошибка: Переменная values не инициализирована.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Console.WriteLine("Инициализация SeriesCollection...");
                var seriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Количество",
                        Values = values
                    }
                };

                GradeDistributionChart.Series = seriesCollection;
                GradeDistributionChart.DataContext = this;
                GradeLabels = labels;

                Console.WriteLine("График успешно настроен.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка со статистикой: {ex.Message}\nStackTrace: {ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string[] GradeLabels { get; set; }
    }
}