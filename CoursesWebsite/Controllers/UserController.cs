using Microsoft.AspNetCore.Mvc;
using CoursesWebsite.Models;
using CoursesWebsite.Data;
using Microsoft.EntityFrameworkCore;
namespace CoursesWebsite.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context = new();

        [HttpGet]
        public IActionResult Register()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult Register(User? user)
        {
            if (user == null)
            {
                return View();
            }
            bool isEmailTaken = _context.Users.Where((e)=>e.Email==user.Email).Any();
            if (ModelState.IsValid && !isEmailTaken)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            else if(isEmailTaken)
            {
                ModelState.AddModelError("Email", "Email is already taken.");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            User? loggedInUser = _context.Users.FirstOrDefault(e => e.Email == user.Email && e.Password == user.Password);
            if (loggedInUser != null)
            {
                HttpContext.Session.SetString("UserId", loggedInUser.Id.ToString());
                HttpContext.Session.SetString("Role", loggedInUser.Role ?? "user");
                HttpContext.Session.SetString("FirstName", loggedInUser.FirstName ?? "");
                HttpContext.Session.SetString("LastName", loggedInUser.LastName ?? "");
                if (loggedInUser.Role == "admin")
                {
                    return RedirectToAction("AdminPanel", "Courses");
                }
                else
                {
                    return RedirectToAction("Index", "Courses");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(user);
        }

        public IActionResult Account()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login");
            }
            Guid userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User? user = _context.Users.Include(e=>e.Enrollments).ThenInclude(e=>e.Course).FirstOrDefault(e => e.Id == userId);
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}
