using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineShop.Models;


namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShoppingContext _context;
        public ProductsController(ShoppingContext context, IConfiguration config)
        {
            _context = context;

        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Discount)
                .Include(p => p.Manufacturer)
                .Include(p => p.Comments)
                .Include(p => p.Ram)
                .Include(p => p.Rom)
                .Include(p => p.Comments).ThenInclude(u => u.User).ThenInclude(o => o.Orders)
                .Include(p => p.Rates)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            product.Comments = product.Comments.OrderByDescending(c => c.CreateAt).ToList();
            product.Rates = product.Rates.OrderByDescending(x => x.CreatAt).ToList();
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> AddCmt(Comment comment)
        {
            if (comment.Cmt == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var user = HttpContext.Session.GetString("User");
            DateTime createDate = DateTime.Now;
            if (user != null)
            {
                User userInfo = JsonConvert.DeserializeObject<User>(user);
                var getUser = _context.Users.FirstOrDefault(x => x.Email == userInfo.Email);
                Comment cmt = new Comment()
                {
                    ProductId = comment.ProductId,
                    UserId = getUser.UserId,
                    Cmt = comment.Cmt,
                    CreateAt = createDate,
                };
                _context.Comments.Add(cmt);
                await _context.SaveChangesAsync();
            }
            else
            {
                return Redirect("~/Users/Login");
            }
            return Ok();
        }
        public async Task<Product> GetProductAsync(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            return (product);
        }

        [HttpPost]
        public async Task<IActionResult> AddCart(int id, int quantity)
        {
            Product product = await GetProductAsync(id);
            var cart = HttpContext.Session.GetString("cart");//Kiểm tra session
            if (cart == null)
            {
                List<Cart> listCart = new List<Cart>()
                {
                   new Cart
                   {
                       product = product,
                       Quantity = 1
                   }
                };
                //HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart, Formatting.Indented, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                }));
            }

            else
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].product.ProductId == product.ProductId)
                    {
                        dataCart[i].Quantity++;
                        check = false;
                    }
                }
                if (check)
                {
                    dataCart.Add(new Cart
                    {
                        product = product,
                        Quantity = 1
                    });
                }
                //HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart, Formatting.Indented, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                }));
            }
            return Ok(quantity);
        }
        [HttpPost]
        public async Task<IActionResult> Rate(Rate rate)
        {
            //var user = HttpContext.Session.GetString("User");
            //User userInfo = JsonConvert.DeserializeObject<User>(user);
            //var getUser = _context.Users.FirstOrDefault(x => x.Email == userInfo.Email);
            DateTime createDate = DateTime.Now;
            Rate UserRate = new Rate()
            {
                UserId = rate.UserId,
                Level = rate.Level,
                Comments = rate.Comments,
                CreatAt = createDate,
                ProductId = rate.ProductId
            };
            _context.Rates.Add(UserRate);

            await _context.SaveChangesAsync();

            return Ok(JsonConvert.SerializeObject(UserRate));
        }
    }
}
