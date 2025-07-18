using System;
using System.Windows;
using System.Windows.Controls;
using StudentPortal.Data;
using Microsoft.Extensions.DependencyInjection;

namespace StudentPortal
{
    public partial class WindowStudents : Window
    {
        private readonly AppDbContext _db;

        public WindowStudents(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            Manager.MainFrame = MainFrame;
            MainFrame.Navigate(new Information(_db));
        }

        private void btngrade_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GradePage(_db));
        }

        private void btncourse_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CoursePage(_db));
        }

        private void btngroup_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GroupPage(_db));
        }

        private void btnstats_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StatisticsPage(_db));
        }

        private void bbtn_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
            else
            {
                var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
                Close();
            }
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            bbtn.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Hidden;
        }
    }
}