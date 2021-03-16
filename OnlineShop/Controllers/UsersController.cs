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
                if (verified)
                {
                    //Save Session
                    User userSession = userForDb;
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
                    return View();
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
                return View();
            }
            else if (user.LastName == null)
            {
                ModelState.AddModelError("LastName", "Vui lòng điền tên");
                return View();
            }
            else if (user.Phone == null)
            {
                ModelState.AddModelError("Phone", "Vui lòng điền số điện thoại");
                return View();
            }
            else if (user.Email == null)
            {
                ModelState.AddModelError("Email", "Vui lòng điền email");
                return View();
            }
            else if (userEmail != null)
            {
                ModelState.AddModelError("Email", "Tài khoản đã tồn tại vui lòng đăng nhập");
                return View();
            }
            else if (user.Phone == null)
            {
                ModelState.AddModelError("Phone", "Vui lòng điền số điện thoại");
                return View();
            }
            else if (user.Password == null)
            {
                ModelState.AddModelError("Password", "Vui lòng điền mật khẩu");
                return View();
            }
            else if (ConfirmPassword == null)
            {
                ModelState.AddModelError("Password", "Vui lòng điền xác nhận mật khẩu");
                return View();
            }
            else if (user.Password != ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Vui lòng kiểm tra lại mật khẩu và xác nhận mật khẩu");
                return View();
            }
            else if (user.Address != null)
            {
                ModelState.AddModelError("Address", "Vui lòng điền số nhà, tên đường");
                return View();
            }
            else if (user.Ward != null)
            {
                ModelState.AddModelError("Address", "Vui lòng điền xã/phường");
                return View();
            }
            else if (user.District != null)
            {
                ModelState.AddModelError("Address", "Vui lòng điền quận/huyện");
                return View();
            }
            else if (user.City != null)
            {
                ModelState.AddModelError("Address", "Vui lòng điền tỉnh/thành Phố");
                return View();
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
                Newuser.Address = user.Address;
                Newuser.Ward = user.Ward;
                Newuser.District = user.District;
                Newuser.City = user.City;
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
            var userForDb = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);

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
                message.From.Add(new MailboxAddress("users project", "aps.netcore.dev@gmail.com"));
                message.To.Add(new MailboxAddress("ASP.NET Shopping", userForDb.Email));
                message.Subject = "users send mail";

                userForDb.Code = randomNumberString;
                _context.Users.Update(userForDb);
                await _context.SaveChangesAsync();

                string myHostUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

                message.Body = new TextPart("html")
                {
                    Text = "<strong>From ASP.NET Shopping</strong>" + "<br>" + "<strong>" + myHostUrl + "Users?UserId=" + user.UserId + "&Code=" + randomNumberString
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
        public async Task<IActionResult> Setting()
        {
            var User = HttpContext.Session.GetString("User");
            var userSession = JsonConvert.DeserializeObject<UserSession>(User);
            var userForDB = await _context.Users.FirstOrDefaultAsync(m => m.UserId == userSession.Id);
            User users = userForDB;
            users.Phone = userForDB.Phone.Trim();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> Setting(User users)
        {
            var User = HttpContext.Session.GetString("User");
            var userSession = JsonConvert.DeserializeObject<UserSession>(User);
            var userForDB = await _context.Users.FirstOrDefaultAsync(m => m.UserId == userSession.Id);
            User usersForDB = userForDB;
            usersForDB.Phone = userForDB.Phone.Trim();

            if (users.FristName == null)
            {
                ModelState.AddModelError("FristName", "Vui lòng điền thông tin");
                return View(usersForDB);
            }
            else if(users.LastName == null)
            {
                ModelState.AddModelError("LastName", "Vui lòng điền thông tin");
                return View(usersForDB);
            }
            else if (users.Phone == null)
            {
                ModelState.AddModelError("Phone", "Vui lòng điền thông tin");
                return View(usersForDB);
            }
            else if (users.Address == null)
            {
                ModelState.AddModelError("Address", "Vui lòng điền thông tin");
                return View(usersForDB);
            }
            else if (users.Ward == null)
            {
                ModelState.AddModelError("Ward", "Vui lòng điền thông tin");
                return View(usersForDB);
            }
            else if (users.District == null)
            {
                ModelState.AddModelError("District", "Vui lòng điền thông tin");
                return View(usersForDB);
            }
            else if (users.City == null)
            {
                ModelState.AddModelError("City", "Vui lòng điền thông tin");
                return View(usersForDB);
            }
            else
            {
                //var User = HttpContext.Session.GetString("User");
                //var userSession = JsonConvert.DeserializeObject<UserSession>(User);
                var UserForDB = await _context.Users.FindAsync(userSession.Id);
                if (UserForDB != null)
                {
                    if (users != null)
                    {
                        UserForDB.FristName = users.FristName;
                        UserForDB.LastName = users.LastName;
                        UserForDB.Phone = users.Phone.Trim();
                        UserForDB.Address = users.Address;
                        UserForDB.Ward = users.Ward;
                        UserForDB.District = users.District;
                        UserForDB.City = users.City;

                        _context.Users.Update(UserForDB);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Setting));
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction(nameof(Login));
        }
    }
}
