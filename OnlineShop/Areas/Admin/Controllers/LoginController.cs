using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public async Task<IActionResult> Login(User user)
        {
            if (user.Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền thông tin");
                return View();

            }
            if (user.Password == null)
            {
                ModelState.AddModelError("Password", "Vui lòng điền thông tin");
                return View();
            }
            var userForDb = _context.Users.SingleOrDefault(x => x.Email == user.Email);
            if (userForDb == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản trong hệ thống vui lòng đăng ký");
                return View();
            }
            else if (userForDb != null)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(user.Password, userForDb.Password);
                if (verified && userForDb.Permission == 1)
                {
                    //Save Session
                    UserSession userSession = new UserSession()
                    {
                        Id = userForDb.UserId,
                        Email = user.Email,
                        Name = userForDb.FristName + " " + userForDb.LastName,
                    };
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(userSession));
                    return Redirect("~/Admin/Dashboard");
                }
                else if (verified)
                {
                    return Redirect("~/");
                }
                else if (verified && userForDb.Permission != 1)
                {
                    ModelState.AddModelError("Email", "Bạn không có quyền truy cập");
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
