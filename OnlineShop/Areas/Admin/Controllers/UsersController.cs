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
    public class UsersController : Controller
    {
        private readonly ShoppingContext _context;

        public UsersController(ShoppingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var shoppingContext = _context.Users.OrderBy(m => m.UserId);
            return View(await shoppingContext.ToListAsync());
        }

    }
}
