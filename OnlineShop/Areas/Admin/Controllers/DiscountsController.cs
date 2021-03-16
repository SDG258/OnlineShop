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
        //public IActionResult Index()
        //{
        //    //DiscountManufacturer discountManufacturer = new DiscountManufacturer();
        //    //var DiscountsContext = _context.Discounts.OrderByDescending(d => d.EndDate);
        //    //var ManufacturerContext = _context.Manufacturers.OrderByDescending(d => d.ManufacturerName);

        //    //discountManufacturer.DiscountModel = DiscountsContext.ToList();
        //    //discountManufacturer.ManufacturerModel = ManufacturerContext.ToList();

        //    //return View(discountManufacturer);
        //    var shoppingContext = _context.Discounts.OrderBy(d => d.AmountOf)/*.Include(x => x.Manufacturer)*/;
        //    return View(shoppingContext.ToListAsync());
        //}
        public async Task<IActionResult> Index()
        {
            var shoppingContext = _context.Discounts.OrderBy(d => d.AmountOf).Include(x => x.Manufacturer);
            return View(await shoppingContext.ToListAsync());
        }

        public async Task<IActionResult> AddDiscount()
        {
            var shoppingContext = _context.Manufacturers.OrderBy(d => d.ManufacturerName);
            return View(await shoppingContext.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> AddDiscount(Discount discount)
        {
            //if (discount.DiscountCode == null)
            //{
            //    ModelState.AddModelError("DiscountCode", "Vui lòng điền trường này");
            //}

            Discount NewDiscount = new Discount();
            NewDiscount.ManufacturerId = discount.ManufacturerId;
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
