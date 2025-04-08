using Microsoft.AspNetCore.Cors; // Add this for CORS
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Profile.Data;
using Profile.Models;
using System;
using System.Linq; // Add this for ToList()
using System.Threading.Tasks;

namespace Profile.Controllers
{
    [EnableCors("AllowLocalhost7276")] // Add this for CORS from your FE
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem hồ sơ.";
                return RedirectToAction("Login", "Login");
            }

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Login", "Login");
            }

            return View(user);
        }

        // POST: /Profile/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(User model, string[] Hobbies)
        {
            var userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để cập nhật hồ sơ.";
                return RedirectToAction("Login", "Login");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users
                        .Include(u => u.Profile)
                        .FirstOrDefaultAsync(u => u.UserID == userId);

                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                        return RedirectToAction("Login", "Login");
                    }

                    // Cập nhật thông tin UserProfile
                    user.Profile.FirstName = model.Profile.FirstName;
                    user.Profile.LastName = model.Profile.LastName;
                    user.Profile.PhoneNumber = model.Profile.PhoneNumber;
                    user.Profile.Hobbies = Hobbies != null && Hobbies.Length > 0
                        ? Hobbies.ToList()
                        : new List<string>();

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật hồ sơ. Vui lòng thử lại.");
                }
            }

            // Nếu lỗi, lấy lại user để hiển thị form
            var currentUser = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Login", "Login");
            }

            TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin.";
            return View("Index", currentUser);
        }

        // New API endpoint: GET /api/profile
        [HttpGet("api/profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdString = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Not logged in");
            }

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                UserID = user.UserID,
                Email = user.Email,
                FirstName = user.Profile?.FirstName,
                LastName = user.Profile?.LastName,
                PhoneNumber = user.Profile?.PhoneNumber,
                Hobbies = user.Profile?.Hobbies ?? new List<string>()
            });
        }
    }
}