using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin
{
    [Area("Admin")]
    public class RamsController : Controller
    {
        private readonly ShoppingContext _context;

        public RamsController(ShoppingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AddRam()
        {
            var shoppingContext = _context.Rams.OrderBy(m => m.Memory);
            return View(await shoppingContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddRam(Ram ram)
        {
            if (ram.Memory == null)
            {
                ModelState.AddModelError("Memory", "Vui lòng điền trường này");
            }
          
            Ram NewRam = new Ram();
            NewRam.Memory = ram.Memory;

            _context.Rams.Update(NewRam);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

    }
}
