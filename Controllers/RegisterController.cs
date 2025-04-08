using Microsoft.AspNetCore.Mvc;
using Profile.Data;
using Profile.Models;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Profile.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Register (Optional, can keep for full-stack testing)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Register (API-friendly version)
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model, [FromForm] string[] Hobbies)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Validation failed", errors = errors });
            }

            // Kiểm tra email trùng lặp
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { success = false, message = "Email đã tồn tại. Vui lòng sử dụng email khác." });
            }

            try
            {
                // Tạo User mới
                var user = new User
                {
                    UserID = Guid.NewGuid(),
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
                };

                // Tạo UserProfile liên kết
                var profile = new UserProfile
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Hobbies = Hobbies != null && Hobbies.Length > 0
                        ? Hobbies.ToList()
                        : new List<string>(),
                    UserID = user.UserID
                };

                user.Profile = profile;

                // Lưu vào database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Đăng ký thành công! Vui lòng đăng nhập." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi khi lưu dữ liệu. Vui lòng thử lại sau." });
            }
        }
    }
}