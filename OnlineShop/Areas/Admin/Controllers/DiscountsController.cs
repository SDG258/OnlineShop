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
    public class DiscountsController : Controller
    {
        private readonly ShoppingContext _context;

        public DiscountsController(ShoppingContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var shoppingContext = _context.Discounts.OrderByDescending(d => d.EndDate);
            return View(await shoppingContext.ToListAsync());
        }
        public async Task<IActionResult> AddDiscount()
        {
            var shoppingContext = _context.Discounts.OrderBy(d => d.EndDate);
            return View(await shoppingContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddDiscount(Discount discount)
        {
            if (discount.DiscountCode == null)
            {
                ModelState.AddModelError("DiscountCode", "Vui lòng điền trường này");
            }

            Discount NewDiscount = new Discount();
            NewDiscount.DiscountCode = discount.DiscountCode;
            NewDiscount.DiscountPercent = discount.DiscountPercent;
            NewDiscount.AmountOf = discount.AmountOf;
            NewDiscount.Used = 0;
            NewDiscount.StartDate = discount.StartDate;
            NewDiscount.EndDate = discount.EndDate;
            NewDiscount.Note = discount.Note;

            _context.Discounts.Update(NewDiscount);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
