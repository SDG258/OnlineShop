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
    public class DashboardController : Controller
    {
        private readonly ShoppingContext _context;

        public DashboardController(ShoppingContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> index()
        {
            return View(await _context.Users.ToListAsync());
        }

    }
}
