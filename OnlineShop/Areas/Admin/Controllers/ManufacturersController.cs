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
    public class ManufacturersController : Controller
    {
        private readonly ShoppingContext _context;

        public ManufacturersController(ShoppingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Manufacturers.OrderBy(x => x.ManufacturerId).ToListAsync());
        }
        public async Task<IActionResult> AddManufacturer()
        {
            var shoppingContext = _context.Manufacturers.OrderBy(d => d.ManufacturerId);
            return View(await shoppingContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddManufacturer(Manufacturer manufacturer)
        {
            if (manufacturer.ManufacturerName == null)
            {
                ModelState.AddModelError("ManufacturerName", "Vui lòng điền trường này");
            }

            Manufacturer NewManufacturer = new Manufacturer();
            NewManufacturer.ManufacturerName = manufacturer.ManufacturerName;

            _context.Manufacturers.Update(NewManufacturer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
