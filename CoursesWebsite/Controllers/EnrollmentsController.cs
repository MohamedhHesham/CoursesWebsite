using CoursesWebsite.Data;
using CoursesWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoursesWebsite.Controllers
{
    public class EnrollmentsController : Controller
    {
        AppDbContext context = new AppDbContext();
        public IActionResult Index(Guid? Id)
        {
            Course course = context.Courses.FirstOrDefault(c => c.Id == Id);
            return View(course);
        }
        [HttpPost]
        public IActionResult Enroll(Guid? Id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "User");
            }

            if (Id == null)
            {
                return BadRequest();
            }

            bool courseState = context.Courses.Any(c => c.Id == Id);
            Guid userId = Guid.Parse(userIdStr);
            bool isEnrolled = context.Enrollments.Where(e => e.CourseId == Id && e.UserId == userId).Any();
            if (courseState && !isEnrolled)
            {
                Enrollment enrollment = new Enrollment();
                enrollment.CourseId = Id;
                enrollment.UserId = userId;
                context.Enrollments.Add(enrollment);
                context.SaveChanges();
                return RedirectToAction("Index", "Courses");
            }
            else if(isEnrolled)
            {
                Course course = context.Courses.FirstOrDefault(c => c.Id == Id);
                ModelState.AddModelError("", "You are already enrolled in this course.");
                return View("Index",course);
            } 
            return BadRequest();
        }

    }
}
