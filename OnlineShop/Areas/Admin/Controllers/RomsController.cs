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
    public class RomsController : Controller
    {
        private readonly ShoppingContext _context;

        public RomsController(ShoppingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AddRom()
        {
            var shoppingContext = _context.Roms.OrderBy(m => m.Space);
            return View(await shoppingContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddRom(Rom rom)
        {
            if (rom.Space == null)
            {
                ModelState.AddModelError("Space", "Vui lòng điền trường này");
            }

            Rom NewRom = new Rom();
            NewRom.Space = rom.Space;

            _context.Roms.Update(NewRom);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}
