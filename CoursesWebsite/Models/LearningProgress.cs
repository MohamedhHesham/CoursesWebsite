using System.ComponentModel.DataAnnotations;

namespace CoursesWebsite.Models
{
    public class LearningProgress
    {
        [Required]
        public Guid? lessonId { get; set; }
        public Lesson? Lesson { get; set; }

        [Required]
        public Guid? userId { get; set; }
        public User? User { get; set; }

        public bool isCompleted { get; set; } = false;
    }
}

