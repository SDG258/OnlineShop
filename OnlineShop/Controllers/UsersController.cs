using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class UsersController : Controller
    {
        private readonly ShoppingContext _context;

        public UsersController(ShoppingContext context)
        {
            _context = context;
        }

        //Đăng nhập
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Login(User user)
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
            var userForDb = _context.Users.SingleOrDefault(x => x.Email == user.Email);
            if (userForDb == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản trong hệ thống vui lòng đăng ký");
            }
            else if (userForDb != null)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(user.Password, userForDb.Password);
                if (verified)
                {
                    //Save Session
                    UserSession userSession = new UserSession()
                    {
                        Id = userForDb.UserId,
                        Email = user.Email,
                    };
                    HttpContext.Session.SetString("User", JsonConvert.SerializeObject(userSession));

                    return Redirect("~/Home/");
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

        //Đăng ký
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user, string ConfirmPassword)
        {
            var userEmail = _context.Users.SingleOrDefault(x => x.Email == user.Email);
            if (user.FristName == null)
            {
                ModelState.AddModelError("FristName", "Vui lòng điền họ");
            }
            else if (user.LastName == null)
            {
                ModelState.AddModelError("LastName", "Vui lòng điền tên");
            }

            else if (user.Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền Mail");

            }
            else if (userEmail != null)
            {
                ModelState.AddModelError("Email", "Tài khoản đã tồn tại vui lòng đăng nhập");
            }
            else if (user.Phone == null)
            {
                ModelState.AddModelError("Phone", "Vui lòng điền số điện thoại");
            }
            else if (user.Password == null)
            {
                ModelState.AddModelError("Password", "Vui lòng điền mật khẩu");
            }
            else if (user.Password != ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Vui lòng kiểm tra lại mật khẩu và xác nhận mật khẩu");
            }
            else
            {
                User Newuser = new User();
                Newuser.FristName = user.FristName;
                Newuser.LastName = user.LastName;
                Newuser.Email = user.Email;
                Newuser.Phone = user.Phone;
                Newuser.Permission = 0;

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                Newuser.Password = passwordHash;
                _context.Users.Add(Newuser);
                await _context.SaveChangesAsync();

                return Redirect("~/");
            }
            return View();
        }
        //Quên mật khẩu 
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(User user)
        {
            var userForDb = _context.Users.FirstOrDefault(x => x.Email == user.Email);

            if (user.Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền thông tin");
            }
            if (userForDb == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản trong hệ thống vui lòng đăng ký");
            }
            else if (userForDb != null)
            {
                Random randomNumber = new Random();
                var randomNumberString = randomNumber.Next(0, 9999).ToString("0000");

                //Send Mail
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Test project", "aps.netcore.dev@gmail.com"));
                message.To.Add(new MailboxAddress("ASP.NET Shopping", userForDb.Email));
                message.Subject = "Test send mail";

                userForDb.Code = randomNumberString;
                _context.Users.Update(userForDb);
                await _context.SaveChangesAsync();

                string myHostUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                message.Body = new TextPart("html")
                {
                    Text = "<strong>From ASP.NET Shopping</strong>" + "<br>" + "<strong>" + myHostUrl + "?UserId=" + user.UserId + "&Code=" + randomNumberString
                };
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("aps.netcore.dev@gmail.com", "Songdu1999#");
                    client.Send(message);
                    client.Disconnect(true);
                }
                return Redirect("~/");
            }
            return View();
        }
        public async Task<IActionResult> Details(int? id, int? code)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Discount)
                .Include(p => p.Manufacturer)
                .Include(p => p.Ram)
                .Include(p => p.Rom)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        //public async Task<IActionResult> GetQueryAsync(int? id, int? code)
        //{
        //    if (id == null && code == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .Include(p => p.Discount)
        //        .Include(p => p.Manufacturer)
        //        .Include(p => p.Ram)
        //        .Include(p => p.Rom)
        //        .FirstOrDefaultAsync(m => m.ProductId == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //[HttpGet("{id}/{code}")]
        //public async Task<IActionResult> GetQueryAsync(int id, string code)
        //{
        //    var user = await _context.Users.FindAsync(id);
        //    if (user != null)
        //    {
        //        if (code == user.Code)
        //        {
        //            user.Code = null;
        //            _context.Users.Update(user);
        //            await _context.SaveChangesAsync();

        //            //Save Session
        //            UserSession userSession = new UserSession()
        //            {
        //                Id = id,
        //                Email = user.Email,
        //            };
        //            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(userSession));

        //            return Redirect("~/Users/ChangePassword");
        //        }
        //    }
        //    return Redirect("~/");
        //}

        //Đổi mật khẩu
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string Password, string ConfirmPassword)
        {
            var User = HttpContext.Session.GetString("User");
            var userSession = JsonConvert.DeserializeObject<UserSession>(User);
            var user = await _context.Users.FindAsync(userSession.Id);
            if (user != null)
            {
                if (Password == ConfirmPassword)
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

                    user.Password = passwordHash;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
