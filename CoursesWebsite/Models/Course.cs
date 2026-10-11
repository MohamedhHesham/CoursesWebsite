using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebsite.Models
{
    public class Course
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Course title is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Course title must be between 3 and 150 characters.")]
        [Display(Name = "Course Title")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Course description is required.")]
        [StringLength(3000, MinimumLength = 10, ErrorMessage = "Course description must be between 10 and 3000 characters.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Course Description")]
        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        [Display(Name = "Course Thumbnail")]
        public IFormFile? Image { get; set; }

        public List<Lesson>? Lessons { get; set; }
        public List<Enrollment>? Enrollments { get; set; }
    }
}

