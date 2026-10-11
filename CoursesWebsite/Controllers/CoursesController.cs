using CoursesWebsite.Data;
using CoursesWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CoursesWebsite.Controllers
{
    public class CoursesController : Controller
    {
        AppDbContext context = new();
        IWebHostEnvironment webHostEnvironment;

        public CoursesController(IWebHostEnvironment webHostEnv)
        {
            webHostEnvironment = webHostEnv;
        }
        public IActionResult Index(string? search)
        {
            if (!search.IsNullOrEmpty())
            {
                ViewBag.currentSearch = search;
                List<Course> courses = context.Courses.Where(e => e.Name.Contains(search) || e.Description.Contains(search)).ToList();
                return View(courses);
            }
            else
            {
                List<Course> courses = context.Courses.ToList();
                return View(courses);
            }

        }
        [HttpGet]
        public IActionResult AddCourse()
        {
            if(HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddCourse(Course course)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }
            if (course == null)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                if (course.Image == null)
                {
                    course.ImagePath = @"\Images\No_Image.png";
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    string imageExtension = Path.GetExtension(course.Image.FileName);
                    course.ImagePath = @"\Images\" + guid + imageExtension;
                    string fullPath = webHostEnvironment.WebRootPath + course.ImagePath;
                    FileStream file = new(fullPath, FileMode.Create);
                    course.Image.CopyTo(file);
                    file.Dispose();
                }
                context.Courses.Add(course);
                context.SaveChanges();
                return RedirectToAction("AdminPanel");
            }
            return View(course);
        }
        public IActionResult AdminPanel()
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }
            List<Course> courses = context.Courses.ToList();
            return View(courses);
        }
        [HttpGet]
        public IActionResult Edit(Guid Id)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }
            Course? course = context.Courses.FirstOrDefault(e=>e.Id == Id);
            return View(course);
        }
        [HttpPost]
        public IActionResult Edit(Course course)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (course.Image != null)
                {
                    Guid guid = Guid.NewGuid();
                    string imageExtension = Path.GetExtension(course.Image.FileName);
                    course.ImagePath = @"\Images\" + guid + imageExtension;
                    string fullPath = webHostEnvironment.WebRootPath + course.ImagePath;
                    FileStream file = new(fullPath, FileMode.Create);
                    course.Image.CopyTo(file);
                    file.Dispose();
                }
                context.Courses.Update(course);
                context.SaveChanges();
                return RedirectToAction("AdminPanel");
            }
            return View(course);


        }
        public IActionResult OpenCourse(Guid? Id)
        {
            bool courseState = context.Courses.Any(c => c.Id == Id);
            Guid userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            bool isEnrolled = context.Enrollments.Where(e => e.CourseId == Id && e.UserId == userId).Any();
            if (courseState && isEnrolled)
            {
                Course course = context.Courses.Include(l=>l.Lessons).FirstOrDefault(c => c.Id == Id);
                return RedirectToAction("Index", "LearningProgress",course);
            }
            else if (!isEnrolled)
            {
                Course course = context.Courses.FirstOrDefault(c => c.Id == Id);
                ModelState.AddModelError("", "You are not enrolled in this course.");
                return View("Index", course);
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult DeleteCourse(Guid id)
        {
            if (HttpContext.Session.GetString("Role") != "admin")
            {
                return NotFound();
            }

            Course? course = context.Courses
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.learningProgress)
                .Include(c => c.Enrollments)
                .FirstOrDefault(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            if (course.Lessons != null && course.Lessons.Any())
            {
                foreach (var lesson in course.Lessons)
                {
                    if (!string.IsNullOrEmpty(lesson.VideoPath) && !lesson.VideoPath.Contains("No_Video"))
                    {
                        var videoFilePath = Path.Combine(webHostEnvironment.WebRootPath, lesson.VideoPath.TrimStart('\\', '/'));
                        if (System.IO.File.Exists(videoFilePath))
                        {
                            try { System.IO.File.Delete(videoFilePath); } catch { }
                        }
                    }

                    if (lesson.learningProgress != null && lesson.learningProgress.Any())
                    {
                        context.LearningProgresses.RemoveRange(lesson.learningProgress);
                    }
                }
                context.Lessons.RemoveRange(course.Lessons);
            }


            if (course.Enrollments != null && course.Enrollments.Any())
            {
                context.Enrollments.RemoveRange(course.Enrollments);
            }

            if (!string.IsNullOrEmpty(course.ImagePath) && !course.ImagePath.Contains("No_Image"))
            {
                var imageFilePath = Path.Combine(webHostEnvironment.WebRootPath, course.ImagePath.TrimStart('\\', '/'));
                if (System.IO.File.Exists(imageFilePath))
                {
                    try { System.IO.File.Delete(imageFilePath); } catch { }
                }
            }


            context.Courses.Remove(course);
            context.SaveChanges();

            return RedirectToAction("AdminPanel");
        }
    }
}
