using CoursesWebsite.Data;
using CoursesWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebsite.Controllers
{
    public class LessonsController : Controller
    {
        IWebHostEnvironment webHostEnvironment;
        AppDbContext context = new();
        public LessonsController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index(Guid? Id)
        {
            Course? course = context.Courses.Include(e=>e.Lessons).FirstOrDefault(e => e.Id == Id);
                ViewBag.CourseId= Id;
            return View(course.Lessons);
        }
        public IActionResult LessonPage(Guid? Id)
        {
            Lesson? lesson = context.Lessons.FirstOrDefault(e => e.Id == Id);
            return View(lesson);
        }
        [HttpGet]
        public IActionResult AddLesson(Guid? Id)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }
            ViewBag.CourseId = Id;
            return View();
        }
        [HttpPost]
        public IActionResult AddLesson(Lesson? lesson)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }
            if (lesson == null)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                lesson.Id = Guid.NewGuid();
                if (lesson.Video == null)
                {
                    lesson.VideoPath = @"\Videos\No_Video.mp4";
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    string videoExtension = Path.GetExtension(lesson.Video.FileName);
                    lesson.VideoPath = @"\Videos\" + guid + videoExtension;
                    string fullPath = webHostEnvironment.WebRootPath+lesson.VideoPath;
                    using FileStream file = new(fullPath, FileMode.Create);
                    lesson.Video.CopyTo(file);
                }
                Course course = context.Courses.Include(e=>e.Lessons).FirstOrDefault(e => e.Id == lesson.CourseId);

                lesson.LessonNumber= course.Lessons.Count() + 1;
                context.Lessons.Add(lesson);
                context.SaveChanges();
                return RedirectToAction("Index", new { Id = lesson.CourseId });
            }
            return View(lesson);
        }

        [HttpPost]
        public IActionResult DeleteLesson(Guid id)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }

            Lesson? lesson = context.Lessons
                .Include(l => l.learningProgress)
                .FirstOrDefault(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound();
            }

            Guid? courseId = lesson.CourseId;

            // 1. Delete physical video file if not default No_Video
            if (!string.IsNullOrEmpty(lesson.VideoPath) && !lesson.VideoPath.Contains("No_Video"))
            {
                var videoFilePath = Path.Combine(webHostEnvironment.WebRootPath, lesson.VideoPath.TrimStart('\\', '/'));
                if (System.IO.File.Exists(videoFilePath))
                {
                    try { System.IO.File.Delete(videoFilePath); } catch { }
                }
            }

            // 2. Remove associated learning progresses
            if (lesson.learningProgress != null && lesson.learningProgress.Any())
            {
                context.LearningProgresses.RemoveRange(lesson.learningProgress);
            }

            // 3. Remove lesson
            context.Lessons.Remove(lesson);
            context.SaveChanges();

            // 4. Re-sequence LessonNumbers for remaining lessons in course
            if (courseId.HasValue)
            {
                var remainingLessons = context.Lessons
                    .Where(l => l.CourseId == courseId.Value)
                    .OrderBy(l => l.LessonNumber)
                    .ToList();

                for (int i = 0; i < remainingLessons.Count; i++)
                {
                    remainingLessons[i].LessonNumber = i + 1;
                }
                context.SaveChanges();
            }

            return RedirectToAction("Index", new { Id = courseId });
        }
    }
}
