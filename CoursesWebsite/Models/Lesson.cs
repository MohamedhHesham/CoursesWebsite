using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebsite.Models
{
    public class Lesson
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Lesson name is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Lesson name must be between 3 and 150 characters.")]
        [Display(Name = "Lesson Title")]
        public string? Name { get; set; }

        [Display(Name = "Lesson Number")]
        public int? LessonNumber { get; set; }

        [Display(Name = "Video File Path")]
        public string? VideoPath { get; set; }

        [NotMapped]
        [Display(Name = "Lesson Video")]
        public IFormFile? Video { get; set; }

        [StringLength(10000, ErrorMessage = "Lesson content cannot exceed 10,000 characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Lesson Content")]
        public string? TextContent { get; set; }

        public Guid? CourseId { get; set; }
        public Course? Course { get; set; }

        public List<LearningProgress>? learningProgress { get; set; }
    }
}

