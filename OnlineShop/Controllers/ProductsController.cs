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

namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShoppingContext _context;

        public ProductsController(ShoppingContext context)
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
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> AddCmt(Comment comment)
        {
            //if (discount.DiscountCode == null)
            //{
            //    ModelState.AddModelError("DiscountCode", "Vui lòng điền trường này");
            //}

            //Discount NewDiscount = new Discount();
            //NewDiscount.DiscountCode = discount.DiscountCode;
            //NewDiscount.DiscountPercent = discount.DiscountPercent;
            //NewDiscount.AmountOf = discount.AmountOf;
            //NewDiscount.StartDate = discount.StartDate;
            //NewDiscount.EndDate = discount.EndDate;

            //_context.Discounts.Update(NewDiscount);
            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        //[HttpPost]
        //public async Task<IActionResult> AddProducts(DetailOrder detailOrder)
        //{

        //    var User = HttpContext.Session.GetString("User");
        //    if(User != null)
        //    {
        //        var userSession = JsonConvert.DeserializeObject<UserSession>(User);
        //        var user = await _context.Users.FindAsync(userSession.Id);
        //        if (user != null)
        //        {
        //            Order order = new Order();
        //            order.UserId = user.UserId;
        //            await _context.Orders.AddAsync(order);
        //            await _context.SaveChangesAsync();

        //            var OrderInDB = _context.Orders
        //                .Where(x => x.OrderId == order.OrderId)
        //                .FirstOrDefault(x => x.OrderId == order.OrderId);
        //            if (OrderInDB != null)
        //            {
        //                DetailOrder detailOrders = new DetailOrder();
        //                detailOrders.ProductId = detailOrder.ProductId;
        //                detailOrders.Price = detailOrder.Price;
        //                DateTime d = DateTime.Now;
        //                detailOrders.DataCreate = d;
        //            }
        //        }
        //    }
        //    return RedirectToAction(nameof(Index));

        //}

        [HttpPost]
        public async Task<IActionResult> AddCart(Product product)
        {
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
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
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
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }
            return Redirect("~/Home/");
        }
        //Hiện thị danh sách giỏ hàng
        public async Task<IActionResult> Cart()
        {
            float tmp = 0;
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;
                    return View(dataCart.ToList());
                }
            }

            return View();
        }
        public IActionResult deleteCart(Product product)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if(dataCart[i].Quantity > 1)
                    {
                        dataCart[i].Quantity -= 1;
                    }
                    else
                    {
                        if (dataCart[i].product.ProductId == product.ProductId)
                        {
                            dataCart.RemoveAt(i);
                        }
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                return RedirectToAction(nameof(Cart));
            }
            return Redirect("~/Home/");
        }
        [HttpPost]
        public async Task<IActionResult> CheckDiscounts(Discount discount, int Sum)
        {
            var discounts = await _context.Discounts.FirstOrDefaultAsync(x => x.DiscountCode == discount.DiscountCode);
            int Total = 0;
            if ( discounts!= null)
            {
                DateTime now = DateTime.Now;
                if (discounts.DiscountCode == discount.DiscountCode && now >= discounts.StartDate && now <= discounts.EndDate && discounts.Used < discounts.AmountOf)
                {
                    Total = Sum - (int)(Sum * (discounts.DiscountPercent / 100));
                    //var discountForDB = await _context.Discounts.FirstOrDefaultAsync(x => x.DiscountCode == discounts.DiscountCode);
                    //discountForDB.Used += 1;
                    //_context.Discounts.Update(discountForDB);
                    //await _context.SaveChangesAsync();
                    TempData["Total"] = Total;

                    return RedirectToAction(nameof(Cart));
                }
            }
            return Redirect("~/Products/Cart");
        }
    }
}
