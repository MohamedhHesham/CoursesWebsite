using CoursesWebsite.Data;
using CoursesWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebsite.Controllers
{
    public class LearningProgressController : Controller
    {
        AppDbContext context = new AppDbContext();
        public IActionResult Index(Course? course)
        {
            if (course == null)
            {
                return BadRequest();
            }
            Guid userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            if (userId == null)
            {
                RedirectToAction("Login", "User");
            }
                List<Lesson> lessons = context.Lessons
                    .Where(l => l.CourseId == course.Id)
                    .Include(l => l.learningProgress
                        .Where(lp => lp.userId == userId))
                    .ToList();



            return View(lessons);
        }
        [HttpPost]
        public IActionResult MarkAsCompleted(Guid id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "User");
            }

            Guid userId = Guid.Parse(userIdStr);

            var lesson = context.Lessons.FirstOrDefault(l => l.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            var progress = context.LearningProgresses
                .FirstOrDefault(lp => lp.lessonId == id && lp.userId == userId);

            if (progress == null)
            {
                progress = new LearningProgress
                {
                    lessonId = id,
                    userId = userId,
                    isCompleted = true
                };
                context.LearningProgresses.Add(progress);
            }
            else
            {
                progress.isCompleted = true;
            }

            context.SaveChanges();
            return RedirectToAction("Index", "LearningProgress", new { Id = lesson.CourseId });
        }
    }
}
