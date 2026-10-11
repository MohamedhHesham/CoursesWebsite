using CoursesWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebsite.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LearningProgress> LearningProgresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=H\\SQLEXPRESS;Database=WebsiteCourse;TrustServerCertificate=True;Trusted_Connection=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LearningProgress>()
                .HasKey(lp => new { lp.lessonId, lp.userId });
        }

    }
}
