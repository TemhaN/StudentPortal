using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentPortal.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentPortal
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _db;

        public MainWindow() : this(App.ServiceProvider.GetRequiredService<AppDbContext>())
        {
        }

        public MainWindow(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            try
            {
                _db.Database.OpenConnection();
                _db.Database.CloseConnection();
            }
            catch (Exception ex)
            {
            }
        }

        private void logbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string log = logbox.Text.Trim();
                string pass = PasswordBox.Password.Trim();
                if (string.IsNullOrEmpty(log) || string.IsNullOrEmpty(pass))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }

                User user = _db.Users.FirstOrDefault(p => p.Login == log && p.Password == pass);
                if (user != null)
                {
                    MessageBox.Show("Вы вошли как " + user.Login);
                    if (user.Role == 1)
                    {
                        var windowStudents = App.ServiceProvider.GetRequiredService<WindowStudents>();
                        windowStudents.Show();
                        this.Close();
                    }
                    else
                    {
                        var userWindow = App.ServiceProvider.GetRequiredService<UserWindow>();
                        userWindow.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Вы ввели неверный Логин/Пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\nInnerException: {ex.InnerException?.Message}");
            }
        }

        private void regbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string log = logbox.Text.Trim();
                string pass = PasswordBox.Password.Trim();

                if (log.Length < 4 || pass.Length < 8)
                {
                    MessageBox.Show("Слишком короткий Логин/Пароль");
                    return;
                }

                User user = _db.Users.FirstOrDefault(p => p.Login == log && p.Password == pass);
                if (user != null)
                {
                    MessageBox.Show("Такой пользователь уже зарегистрирован!");
                    return;
                }

                User newUser = new User { Login = log, Password = pass, Role = 0 };
                _db.Users.Add(newUser);
                _db.SaveChanges();
                MessageBox.Show("Добро пожаловать, " + newUser.Login);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\nInnerException: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\nInnerException: {ex.InnerException?.Message}");
            }
        }
    }
}