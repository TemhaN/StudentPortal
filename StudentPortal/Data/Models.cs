using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required, MaxLength(50)]
        public string Login { get; set; }
        [Required, MaxLength(100)]
        public string Password { get; set; }
        public int Role { get; set; } 
    }

    public class Student
    {
        public int StudentId { get; set; }
        public string Imya { get; set; }
        public string Familiya { get; set; }
        public string Otchestvo { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        [StringLength(int.MaxValue)] 
        public string? Photo { get; set; }
        public ICollection<GradeStudent> GradeStudents { get; set; }
    }

    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Required, MaxLength(50)]
        public string GroupName { get; set; }
        public int CourseId { get; set; }
        public int YearStart { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Subject> Subjects { get; set; }
    }

    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Required, MaxLength(100)]
        public string CourseName { get; set; }
        public int Duration { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public ICollection<Group> Groups { get; set; }
    }

    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }
        [Required, MaxLength(100)]
        public string SubjectName { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }

    public class Grade
    {
        [Key]
        public int GradeId { get; set; }
        public int GradeValue { get; set; }
        public int SubjectId { get; set; }
        public DateTime GradeDate { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        public ICollection<GradeStudent> GradeStudents { get; set; }
    }

    public class GradeStudent
    {
        [Column(Order = 1)]
        public int StudentId { get; set; }
        [Column(Order = 2)]
        public int GradeId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        [ForeignKey("GradeId")]
        public Grade Grade { get; set; }
    }
}