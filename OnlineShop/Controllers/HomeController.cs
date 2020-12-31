using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShop.Models;
using PagedList;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShoppingContext _context;

        public HomeController(ShoppingContext context)
        {
            _context = context;
        }

        //Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var shoppingContext = _context.Products.OrderByDescending(m => m.ProductId).Include(o => o.Rom).Include(a => a.Ram).Include(m => m.Manufacturer).Include(d => d.Discount).Include(w => w.WareHouses);
            return View(await shoppingContext.ToListAsync());
        }
        //public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        //{
        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.ProductIdSortParm = String.IsNullOrEmpty(sortOrder) ? "ProductId_desc" : "";
        //    ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_desc" : "Price";
        //    ViewBag.NameProductSortParm = sortOrder == "NameProduct" ? "NameProduct_desc" : "NameProduct";

        //    if (searchString != null)
        //    {
        //        page = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }

        //    ViewBag.CurrentFilter = searchString;

        //    var products = _context.Products.Include(o => o.Rom).Include(a => a.Ram).Include(m => m.Manufacturer).Include(d => d.Discount).OrderByDescending(x => x.ProductId).AsQueryable();

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        products = products.Where(s => s.NameProduct.Contains(searchString)
        //                               || s.Manufacturer.ManufacturerName.Contains(searchString));
        //    }

        //    switch (sortOrder)
        //    {
        //        case "ProductId_desc":
        //            products = products.OrderByDescending(x => x.ProductId);
        //            break;
        //        case "Price":
        //            products = products.OrderBy(x => x.Price);
        //            break;
        //        case "Price_desc":
        //            products = products.OrderByDescending(x => x.Price);
        //            break;
        //        default:
        //            products = products.OrderBy(x => x.ProductId);
        //            break;
        //    }
        //    int pageSize = 3;
        //    int pageNumber = (page ?? 1);
        //    return View(products.ToPagedList(pageNumber, pageSize));
        //}

        public IActionResult Login()
        {
            return View();
        }
    }
}
