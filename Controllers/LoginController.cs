using Microsoft.AspNetCore.Mvc;
using Profile.Data;
using Profile.Models;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Profile.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Login (Optional, can keep for full-stack testing)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Login (API-friendly version)
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Validation failed", errors = errors });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                HttpContext.Session.SetString("UserID", user.UserID.ToString());
                return Ok(new { success = true, message = "Đăng nhập thành công!" });
            }

            return BadRequest(new { success = false, message = "Email hoặc mật khẩu không đúng." });
        }

        [HttpPost("api/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear the session
            return Ok(new { success = true, message = "Logged out successfully" });
        }
    }
}