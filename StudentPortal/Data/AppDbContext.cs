using Microsoft.EntityFrameworkCore;

namespace StudentPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<GradeStudent> GradeStudents { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Subject>().ToTable("Subjects");
            modelBuilder.Entity<Grade>().ToTable("Grades");
            modelBuilder.Entity<GradeStudent>().ToTable("GradeStudents");
            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<GradeStudent>()
                .HasKey(gs => new { gs.StudentId, gs.GradeId });

            modelBuilder.Entity<GradeStudent>()
                .HasOne(gs => gs.Student)
                .WithMany(s => s.GradeStudents)
                .HasForeignKey(gs => gs.StudentId);

            modelBuilder.Entity<Student>()
                .Property(s => s.Photo)
                .IsRequired(false);

            modelBuilder.Entity<GradeStudent>()
                .HasOne(gs => gs.Grade)
                .WithMany(g => g.GradeStudents)
                .HasForeignKey(gs => gs.GradeId);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Subjects)
                .WithMany(s => s.Groups)
                .UsingEntity(j => j.ToTable("GroupSubjects"));

            modelBuilder.Ignore<System.Windows.Media.Animation.DecimalKeyFrameCollection>();
            modelBuilder.Ignore<System.Windows.Media.Animation.Int32KeyFrameCollection>();
            modelBuilder.Ignore<System.Windows.Media.Animation.Int64KeyFrameCollection>();
            modelBuilder.Ignore<System.Windows.Media.Animation.MatrixKeyFrameCollection>();
            modelBuilder.Ignore<System.Windows.Media.Animation.ObjectKeyFrameCollection>();
            modelBuilder.Ignore<System.Windows.Media.Animation.Vector3DKeyFrameCollection>();
            modelBuilder.Ignore<System.Windows.Media.Animation.ThicknessKeyFrameCollection>();
            modelBuilder.Ignore<System.Reflection.AssemblyName>();

            base.OnModelCreating(modelBuilder);
        }
    }
}