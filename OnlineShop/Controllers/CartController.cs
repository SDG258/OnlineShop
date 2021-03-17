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
    public class CartController : Controller
    {
        private readonly ShoppingContext _context;

        public CartController(ShoppingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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

        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].product.ProductId == id && dataCart[i].Quantity > 1)
                    {
                        dataCart[i].Quantity -= 1;
                    }

                    else if (dataCart[i].product.ProductId == id && dataCart[i].Quantity == 1)
                    {
                        dataCart.RemoveAt(i);
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                }));
                return Ok(dataCart.Count);
            }
            return Ok();
        }
    }
}
