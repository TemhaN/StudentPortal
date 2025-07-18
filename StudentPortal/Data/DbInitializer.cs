using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using System;
using System.Linq;

namespace StudentPortal
{
    public static class DbInitializer
    {
        private const string DefaultPhoto = "";

        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Проверка, заполнена ли БД
            if (context.Courses.Any())
            {
                return; // БД уже заполнена
            }

            // Курсы
            var courses = new Course[]
            {
                new Course { CourseName = "Информатика", Duration = 4, Description = "Бакалавриат по компьютерным наукам" },
                new Course { CourseName = "Математика", Duration = 4, Description = "Бакалавриат по математике" },
                new Course { CourseName = "Физика", Duration = 4, Description = "Бакалавриат по физике" }
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();

            // Группы
            var groups = new Group[]
            {
                new Group { GroupName = "ИТ-101", CourseId = courses[0].CourseId, YearStart = 2023 },
                new Group { GroupName = "ИТ-102", CourseId = courses[0].CourseId, YearStart = 2023 },
                new Group { GroupName = "МАТ-101", CourseId = courses[1].CourseId, YearStart = 2023 },
                new Group { GroupName = "МАТ-102", CourseId = courses[1].CourseId, YearStart = 2023 },
                new Group { GroupName = "ФИЗ-101", CourseId = courses[2].CourseId, YearStart = 2023 },
                new Group { GroupName = "ФИЗ-102", CourseId = courses[2].CourseId, YearStart = 2023 }
            };
            context.Groups.AddRange(groups);
            context.SaveChanges();

            // Предметы
            var subjects = new Subject[]
            {
                new Subject { SubjectName = "Программирование на C#", Description = "Основы программирования" },
                new Subject { SubjectName = "Базы данных", Description = "Реляционные БД и SQL" },
                new Subject { SubjectName = "Алгоритмы", Description = "Алгоритмы и структуры данных" },
                new Subject { SubjectName = "Сети", Description = "Компьютерные сети" },
                new Subject { SubjectName = "Линейная алгебра", Description = "Матрицы и векторы" },
                new Subject { SubjectName = "Математический анализ", Description = "Дифференцирование и интегрирование" },
                new Subject { SubjectName = "Дискретная математика", Description = "Теория графов и комбинаторика" },
                new Subject { SubjectName = "Механика", Description = "Классическая механика" },
                new Subject { SubjectName = "Электродинамика", Description = "Электричество и магнетизм" },
                new Subject { SubjectName = "Квантовая физика", Description = "Основы квантовой механики" }
            };
            context.Subjects.AddRange(subjects);
            context.SaveChanges();

            // Связь Группы-Предметы
            var groupSubjects = new[]
            {
                new { GroupId = groups[0].GroupId, SubjectId = subjects[0].SubjectId }, // ИТ-101: Программирование
                new { GroupId = groups[0].GroupId, SubjectId = subjects[1].SubjectId }, // ИТ-101: Базы данных
                new { GroupId = groups[0].GroupId, SubjectId = subjects[2].SubjectId }, // ИТ-101: Алгоритмы
                new { GroupId = groups[0].GroupId, SubjectId = subjects[3].SubjectId }, // ИТ-101: Сети
                new { GroupId = groups[1].GroupId, SubjectId = subjects[0].SubjectId }, // ИТ-102: Программирование
                new { GroupId = groups[1].GroupId, SubjectId = subjects[1].SubjectId }, // ИТ-102: Базы данных
                new { GroupId = groups[1].GroupId, SubjectId = subjects[3].SubjectId }, // ИТ-102: Сети
                new { GroupId = groups[2].GroupId, SubjectId = subjects[4].SubjectId }, // МАТ-101: Линейная алгебра
                new { GroupId = groups[2].GroupId, SubjectId = subjects[5].SubjectId }, // МАТ-101: Мат. анализ
                new { GroupId = groups[2].GroupId, SubjectId = subjects[6].SubjectId }, // МАТ-101: Дискр. математика
                new { GroupId = groups[3].GroupId, SubjectId = subjects[5].SubjectId }, // МАТ-102: Мат. анализ
                new { GroupId = groups[3].GroupId, SubjectId = subjects[6].SubjectId }, // МАТ-102: Дискр. математика
                new { GroupId = groups[4].GroupId, SubjectId = subjects[7].SubjectId }, // ФИЗ-101: Механика
                new { GroupId = groups[4].GroupId, SubjectId = subjects[8].SubjectId }, // ФИЗ-101: Электродинамика
                new { GroupId = groups[5].GroupId, SubjectId = subjects[8].SubjectId }, // ФИЗ-102: Электродинамика
                new { GroupId = groups[5].GroupId, SubjectId = subjects[9].SubjectId }  // ФИЗ-102: Квантовая физика
            };
            foreach (var gs in groupSubjects)
            {
                context.Database.ExecuteSqlRaw("INSERT INTO GroupSubjects (GroupsGroupId, SubjectsSubjectId) VALUES ({0}, {1})", gs.GroupId, gs.SubjectId);
            };

            // Студенты
            var students = new Student[]
            {
                new Student { Imya = "Иван", Familiya = "Иванов", Otchestvo = "Иванович", DateOfBirth = new DateTime(2000, 5, 15), Email = "ivanov@example.com", PhoneNumber = "+79123456789", GroupId = groups[0].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Анна", Familiya = "Петрова", Otchestvo = "Сергеевна", DateOfBirth = new DateTime(2001, 3, 22), Email = "petrova@example.com", PhoneNumber = "+79123456790", GroupId = groups[0].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Мария", Familiya = "Сидорова", Otchestvo = "Алексеевна", DateOfBirth = new DateTime(2000, 7, 10), Email = "sidorova@example.com", PhoneNumber = "+79123456791", GroupId = groups[1].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Петр", Familiya = "Кузнецов", Otchestvo = "Михайлович", DateOfBirth = new DateTime(2001, 1, 30), Email = "kuznetsov@example.com", PhoneNumber = "+79123456792", GroupId = groups[1].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Елена", Familiya = "Смирнова", Otchestvo = "Владимировна", DateOfBirth = new DateTime(2000, 9, 5), Email = "smirnova@example.com", PhoneNumber = "+79123456793", GroupId = groups[2].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Алексей", Familiya = "Васильев", Otchestvo = "Дмитриевич", DateOfBirth = new DateTime(2001, 11, 12), Email = "vasiliev@example.com", PhoneNumber = "+79123456794", GroupId = groups[2].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Ольга", Familiya = "Морозова", Otchestvo = "Павловна", DateOfBirth = new DateTime(2000, 4, 20), Email = "morozova@example.com", PhoneNumber = "+79123456795", GroupId = groups[3].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Дмитрий", Familiya = "Попов", Otchestvo = "Александрович", DateOfBirth = new DateTime(2001, 6, 25), Email = "popov@example.com", PhoneNumber = "+79123456796", GroupId = groups[3].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Сергей", Familiya = "Федоров", Otchestvo = "Игоревич", DateOfBirth = new DateTime(2000, 2, 14), Email = "fedorov@example.com", PhoneNumber = "+79123456797", GroupId = groups[0].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Татьяна", Familiya = "Лебедева", Otchestvo = "Николаевна", DateOfBirth = new DateTime(2001, 8, 18), Email = "lebedeva@example.com", PhoneNumber = "+79123456798", GroupId = groups[1].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Михаил", Familiya = "Григорьев", Otchestvo = "Сергеевич", DateOfBirth = new DateTime(2000, 10, 3), Email = "grigoryev@example.com", PhoneNumber = "+79123456799", GroupId = groups[2].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Екатерина", Familiya = "Козлова", Otchestvo = "Андреевна", DateOfBirth = new DateTime(2001, 12, 7), Email = "kozlova@example.com", PhoneNumber = "+79123456800", GroupId = groups[3].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Андрей", Familiya = "Новиков", Otchestvo = "Викторович", DateOfBirth = new DateTime(2000, 3, 9), Email = "novikov@example.com", PhoneNumber = "+79123456801", GroupId = groups[4].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Юлия", Familiya = "Михайлова", Otchestvo = "Олеговна", DateOfBirth = new DateTime(2001, 5, 16), Email = "mikhaylova@example.com", PhoneNumber = "+79123456802", GroupId = groups[4].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Владимир", Familiya = "Орлов", Otchestvo = "Петрович", DateOfBirth = new DateTime(2000, 7, 22), Email = "orlov@example.com", PhoneNumber = "+79123456803", GroupId = groups[5].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Наталья", Familiya = "Романова", Otchestvo = "Ивановна", DateOfBirth = new DateTime(2001, 9, 28), Email = "romanova@example.com", PhoneNumber = "+79123456804", GroupId = groups[5].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Игорь", Familiya = "Соколов", Otchestvo = "Михайлович", DateOfBirth = new DateTime(2000, 11, 4), Email = "sokolov@example.com", PhoneNumber = "+79123456805", GroupId = groups[0].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Анастасия", Familiya = "Коваленко", Otchestvo = "Владимировна", DateOfBirth = new DateTime(2001, 1, 19), Email = "kovalenko@example.com", PhoneNumber = "+79123456806", GroupId = groups[1].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Виктор", Familiya = "Зайцев", Otchestvo = "Алексеевич", DateOfBirth = new DateTime(2000, 4, 25), Email = "zaytsev@example.com", PhoneNumber = "+79123456807", GroupId = groups[2].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Светлана", Familiya = "Беляева", Otchestvo = "Дмитриевна", DateOfBirth = new DateTime(2001, 6, 30), Email = "belyaeva@example.com", PhoneNumber = "+79123456808", GroupId = groups[3].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Артём", Familiya = "Крылов", Otchestvo = "Сергеевич", DateOfBirth = new DateTime(2000, 8, 12), Email = "krylov@example.com", PhoneNumber = "+79123456809", GroupId = groups[4].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Ксения", Familiya = "Голубева", Otchestvo = "Игоревна", DateOfBirth = new DateTime(2001, 2, 17), Email = "golubeva@example.com", PhoneNumber = "+79123456810", GroupId = groups[5].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Роман", Familiya = "Егоров", Otchestvo = "Викторович", DateOfBirth = new DateTime(2000, 10, 15), Email = "egorov@example.com", PhoneNumber = "+79123456811", GroupId = groups[0].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Вера", Familiya = "Фомина", Otchestvo = "Александровна", DateOfBirth = new DateTime(2001, 4, 22), Email = "fomina@example.com", PhoneNumber = "+79123456812", GroupId = groups[1].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Николай", Familiya = "Богданов", Otchestvo = "Петрович", DateOfBirth = new DateTime(2000, 6, 8), Email = "bogdanov@example.com", PhoneNumber = "+79123456813", GroupId = groups[2].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Алина", Familiya = "Савельева", Otchestvo = "Михайловна", DateOfBirth = new DateTime(2001, 7, 19), Email = "saveleva@example.com", PhoneNumber = "+79123456814", GroupId = groups[3].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Глеб", Familiya = "Титов", Otchestvo = "Иванович", DateOfBirth = new DateTime(2000, 12, 25), Email = "titov@example.com", PhoneNumber = "+79123456815", GroupId = groups[4].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Дарья", Familiya = "Шарова", Otchestvo = "Сергеевна", DateOfBirth = new DateTime(2001, 3, 14), Email = "sharova@example.com", PhoneNumber = "+79123456816", GroupId = groups[5].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Егор", Familiya = "Лазарев", Otchestvo = "Алексеевич", DateOfBirth = new DateTime(2000, 5, 30), Email = "lazarev@example.com", PhoneNumber = "+79123456817", GroupId = groups[0].GroupId, Photo = DefaultPhoto },
                new Student { Imya = "Полина", Familiya = "Виноградова", Otchestvo = "Дмитриевна", DateOfBirth = new DateTime(2001, 9, 10), Email = "vinogradova@example.com", PhoneNumber = "+79123456818", GroupId = groups[1].GroupId, Photo = DefaultPhoto }
            };
            context.Students.AddRange(students);
            context.SaveChanges();

            // Пользователи
            var users = new User[]
            {
                new User { Login = "ivanov", Password = "pass123", Role = 3 },
                new User { Login = "petrova", Password = "pass456", Role = 3 },
                new User { Login = "sidorova", Password = "pass789", Role = 3 },
                new User { Login = "kuznetsov", Password = "pass101", Role = 3 },
                new User { Login = "smirnova", Password = "pass112", Role = 3 },
                new User { Login = "vasiliev", Password = "pass131", Role = 3 },
                new User { Login = "morozova", Password = "pass415", Role = 3 },
                new User { Login = "popov", Password = "pass161", Role = 3 },
                new User { Login = "fedorov", Password = "pass718", Role = 3 },
                new User { Login = "lebedeva", Password = "pass920", Role = 3 },
                new User { Login = "admin", Password = "admin", Role = 1 }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // Оценки
            var random = new Random();
            var grades = new Grade[100];
            for (int i = 0; i < grades.Length; i++)
            {
                var student = students[random.Next(students.Length)];
                var group = groups.First(g => g.GroupId == student.GroupId);
                var groupSubjectIds = groupSubjects.Where(gs => gs.GroupId == group.GroupId).Select(gs => gs.SubjectId).ToList();
                var subjectId = groupSubjectIds[random.Next(groupSubjectIds.Count)];

                grades[i] = new Grade
                {
                    GradeValue = random.Next(2, 6), // Оценки 2–5
                    SubjectId = subjectId,
                    GradeDate = DateTime.Now.AddDays(-random.Next(1, 180)) // Оценки за последние полгода
                };
            }
            context.Grades.AddRange(grades);
            context.SaveChanges();

            // Связь Оценки-Студенты
            foreach (var grade in grades)
            {
                var student = students[random.Next(students.Length)];
                var group = groups.First(g => g.GroupId == student.GroupId);
                var groupSubjectIds = groupSubjects.Where(gs => gs.GroupId == group.GroupId).Select(gs => gs.SubjectId).ToList();
                if (groupSubjectIds.Contains(grade.SubjectId))
                {
                    context.GradeStudents.Add(new GradeStudent
                    {
                        StudentId = student.StudentId,
                        GradeId = grade.GradeId
                    });
                }
            }
            context.SaveChanges();
        }
    }
}