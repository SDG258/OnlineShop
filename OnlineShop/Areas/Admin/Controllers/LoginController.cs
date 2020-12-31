using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly ShoppingContext _context;

        public LoginController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Admin/Login
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HandleLogin(User user)
        {
            if (user.Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền thông tin");
                return RedirectToAction(nameof(Index));

            }
            if (user.Password == null)
            {
                ModelState.AddModelError("Password", "Vui lòng điền thông tin");
                return RedirectToAction(nameof(Index));
            }
            var userForDb = _context.Users.FirstOrDefault(x => x.Email == user.Email);
            if (userForDb == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản trong hệ thống vui lòng đăng ký");
            }
            else if (userForDb != null)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(user.Password, userForDb.Password);
                if (verified && userForDb.Permission == 1)
                {
                    return Redirect("~/Admin/Dashboard");
                }
                else if (verified)
                {
                    return Redirect("~/");
                }
                else
                {
                    ModelState.AddModelError("Password", "Vui lòng kiểm tra lại mật khẩu");
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
