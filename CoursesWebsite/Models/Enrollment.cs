using System.ComponentModel.DataAnnotations;

namespace CoursesWebsite.Models
{
    public class Enrollment
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid? CourseId { get; set; }
        public Course? Course { get; set; }

        [Required]
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}

