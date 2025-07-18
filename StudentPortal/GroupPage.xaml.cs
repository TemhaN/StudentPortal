using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentPortal
{
    public partial class GroupPage : Page // Переименовано из Group
    {
        private readonly AppDbContext _db;

        public GroupPage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadGroups();
            LoadCourses();
        }

        private void LoadGroups()
        {
            ulist.ItemsSource = _db.Groups.ToList();
        }

        private void LoadCourses()
        {
            comborole.ItemsSource = _db.Courses.ToList();
            comborole.DisplayMemberPath = "CourseName";
            comborole.SelectedValuePath = "CourseId";
        }

        private void ulist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ulist.SelectedItem is StudentPortal.Group selectedGroup) // Явно указано StudentPortal.Group
            {
                nametext.Text = selectedGroup.GroupName;
                comborole.SelectedValue = selectedGroup.CourseId;
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ulist.SelectedItem is StudentPortal.Group selectedGroup) // Явно указано StudentPortal.Group
                {
                    selectedGroup.GroupName = nametext.Text;
                    selectedGroup.CourseId = (int)comborole.SelectedValue;
                    _db.Update(selectedGroup);
                }
                else
                {
                    var newGroup = new StudentPortal.Group // Явно указано StudentPortal.Group
                    {
                        GroupName = nametext.Text,
                        CourseId = (int)comborole.SelectedValue,
                        YearStart = DateTime.Now.Year
                    };
                    _db.Groups.Add(newGroup);
                }

                _db.SaveChanges();
                LoadGroups();
                ClearInputFields();
                MessageBox.Show("Данные сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\nInnerException: {ex.InnerException?.Message}");
            }
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ulist.SelectedItem is StudentPortal.Group selectedGroup) // Явно указано StudentPortal.Group
                {
                    _db.Groups.Remove(selectedGroup);
                    _db.SaveChanges();
                    LoadGroups();
                    ClearInputFields();
                    MessageBox.Show("Группа удалена");
                }
                else
                {
                    MessageBox.Show("Выберите группу для удаления");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\nInnerException: {ex.InnerException?.Message}");
            }
        }

        private void cleanb_Click(object sender, RoutedEventArgs e)
        {
            ClearInputFields();
        }

        private void ClearInputFields()
        {
            nametext.Text = "";
            comborole.SelectedItem = null;
            ulist.SelectedItem = null;
        }
    }
}