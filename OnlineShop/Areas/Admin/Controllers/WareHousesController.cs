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
    public class WareHousesController : Controller
    {
        private readonly ShoppingContext _context;

        public WareHousesController(ShoppingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var shoppingContext = _context.WareHouses.OrderBy(n => n.WareHouseId).Include(w => w.Product);
            return View(await shoppingContext.ToListAsync());
        }
    }
}
