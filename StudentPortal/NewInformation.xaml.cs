using DocumentFormat.OpenXml.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using StudentPortal.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StudentPortal
{
    public partial class NewInformation : Page
    {
        private readonly AppDbContext _db;
        private Student _currentStudent;
        public Information PreviousPage { get; set; }
        public event EventHandler StudentSaved;

        public NewInformation(AppDbContext db, Student student, Information previousPage)
        {
            InitializeComponent();
            _db = db;
            _currentStudent = student ?? new Student();
            DataContext = _currentStudent;
            Combogroup.ItemsSource = _db.Groups.ToList();
            PreviousPage = previousPage;

            if (_currentStudent.StudentId != 0 && !string.IsNullOrEmpty(_currentStudent.Photo))
            {
                try
                {
                    byte[] photoBytes = Convert.FromBase64String(_currentStudent.Photo);
                    using (MemoryStream ms = new MemoryStream(photoBytes))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        Photo.Source = bitmap;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки фотографии из базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private byte[] CompressImage(string filePath, long targetSizeBytes = 500 * 1024)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Файл изображения не существует.");

                using (var image = System.Drawing.Image.FromFile(filePath))
                {
                    if (image.RawFormat.Guid != ImageFormat.Jpeg.Guid && image.RawFormat.Guid != ImageFormat.Png.Guid)
                        throw new InvalidOperationException("Поддерживаются только форматы JPEG и PNG.");

                    int width = image.Width;
                    int height = image.Height;

                    // Уменьшаем до 800x600
                    double scaleX = 800.0 / width;
                    double scaleY = 600.0 / height;
                    double scale = Math.Min(scaleX, scaleY);

                    var resizedImage = new Bitmap((int)(width * scale), (int)(height * scale));
                    using (var graphics = Graphics.FromImage(resizedImage))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, resizedImage.Width, resizedImage.Height);
                    }

                    using (var ms = new MemoryStream())
                    {
                        // Сохраняем как JPEG
                        var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                        var encParams = new EncoderParameters(1);
                        encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L); // 85% качества
                        resizedImage.Save(ms, encoder, encParams);

                        if (ms.Length > targetSizeBytes)
                        {
                            throw new InvalidOperationException($"Размер изображения ({ms.Length} байт) превышает целевой ({targetSizeBytes} байт). Попробуйте файл меньшего размера.");
                        }

                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Не удалось сжать изображение: {ex.Message}", ex);
            }
        }
        private void SaveJpeg(System.Drawing.Image image, Stream stream, byte quality)
        {
            var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encParams = new EncoderParameters(1);
            encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            image.Save(stream, encoder, encParams);
        }

        private byte[] ConvertImageToByteArray(BitmapImage bitmap)
        {
            using (var ms = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(_currentStudent.Imya)) errors.AppendLine("Введите имя.");
            if (string.IsNullOrEmpty(_currentStudent.Familiya)) errors.AppendLine("Введите фамилию.");
            if (string.IsNullOrEmpty(_currentStudent.Otchestvo)) errors.AppendLine("Введите отчество.");
            if (_currentStudent.DateOfBirth == null) errors.AppendLine("Укажите дату рождения.");
            if (_currentStudent.DateOfBirth != null && _currentStudent.DateOfBirth > DateTime.Today)
                errors.AppendLine("Дата рождения не может быть в будущем.");
            if (string.IsNullOrEmpty(_currentStudent.Email)) errors.AppendLine("Введите email.");
            if (!string.IsNullOrEmpty(_currentStudent.Email) && !_currentStudent.Email.Contains("@"))
                errors.AppendLine("Введите корректный email.");
            if (!string.IsNullOrEmpty(_currentStudent.Email) && _db.Students.Any(s => s.Email == _currentStudent.Email && s.StudentId != _currentStudent.StudentId))
                errors.AppendLine("Этот email уже используется.");
            if (string.IsNullOrEmpty(_currentStudent.PhoneNumber)) errors.AppendLine("Введите номер телефона.");

            var selectedGroup = Combogroup.SelectedItem as Group;
            if (selectedGroup == null)
            {
                errors.AppendLine("Выберите группу.");
            }
            else
            {
                _currentStudent.GroupId = selectedGroup.GroupId;
                if (!_db.Groups.Any(g => g.GroupId == _currentStudent.GroupId))
                {
                    errors.AppendLine("Выбранная группа не существует в базе данных.");
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            byte[] photoBytes = null;
            if (Photo.Source != null && Photo.Source is BitmapImage bitmapImage)
            {
                try
                {
                    photoBytes = ConvertImageToByteArray(bitmapImage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении фотографии: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }


            try
            {
                await Task.Run(() =>
                {
                    if (photoBytes != null)
                    {
                        string photoBase64 = Convert.ToBase64String(photoBytes);
                        _currentStudent.Photo = photoBase64;
                    }
                    else
                    {
                        _currentStudent.Photo = null;
                    }

                    if (_currentStudent.StudentId == 0)
                        _db.Students.Add(_currentStudent);
                    else
                        _db.Update(_currentStudent);

                    _db.SaveChanges();
                });

                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                StudentSaved?.Invoke(this, EventArgs.Empty);
                PreviousPage.LoadStudents();
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\nInnerException: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    Console.WriteLine($"Выбран файл: {fileInfo.FullName}, Размер={fileInfo.Length} байт");
                    if (fileInfo.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("Файл слишком большой, максимум 5 МБ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    byte[] compressedBytes = CompressImage(openFileDialog.FileName, 200 * 1024);

                    using (MemoryStream ms = new MemoryStream(compressedBytes))
                    {
                        ms.Position = 0; // Сбрасываем позицию потока
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze(); // Замораживаем для потокобезопасности
                        Photo.Source = bitmap;
                    }

                    string photoBase64 = Convert.ToBase64String(compressedBytes);
                    _currentStudent.Photo = photoBase64;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при выборе фотографии: {ex.Message}\nВнутренняя ошибка: {ex.InnerException?.Message}\nСтек вызовов: {ex.StackTrace}",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Combogroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}