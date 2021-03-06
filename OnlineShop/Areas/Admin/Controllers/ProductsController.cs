﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ShoppingContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;


        public ProductsController(ShoppingContext context, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            _context = context;
        }

        //Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var shoppingContext = _context.WareHouses.OrderByDescending(m => m.Date).Include(p => p.Product).Include(m => m.Product.Manufacturer).Include(a => a.Product.Ram).Include(o => o.Product.Rom);
            return View(await shoppingContext.ToListAsync());
            //ListProductInWaewHouse listProductInWaewHouse = new ListProductInWaewHouse();
            //var WareHouseContext = _context.WareHouses.OrderBy(m => m.Date).Include(o => o.Product).Include(m => m.Product.Manufacturer).Include(a => a.Product.Ram).Include(o => o.Product.Rom).ToList();
            //var ProductContext = _context.Products.OrderBy(m => m.ProductId).Include(m => m.Manufacturer);

            //listProductInWaewHouse.WareHouseiewModel = WareHouseContext.ToList();
            //listProductInWaewHouse.ProductViewModel = ProductContext.ToList();

            //return View(listProductInWaewHouse);

        }
        //Thêm sảm phẩm
        public async Task<IActionResult> AddProduct()
        {
            InfoProduct infoProduct = new InfoProduct();
            var ManufacturersContext = _context.Manufacturers.OrderBy(m => m.ManufacturerId).ToList();
            var DiscountsContext = _context.Discounts.OrderBy(m => m.EndDate);
            var RamContext = _context.Rams.OrderBy(m => m.Memory);
            var RomContext = _context.Roms.OrderBy(m => m.Space);

            infoProduct.ManufacturerViewModel = ManufacturersContext.ToList();
            infoProduct.DiscountViewModel = DiscountsContext.ToList();
            infoProduct.RamViewModel = RamContext.ToList();
            infoProduct.RomViewModel = RomContext.ToList();
            return View(infoProduct);
        }
        private async Task<string> GetUniqueFileName(ProImgAndModelduct product, string fileName)
        {
            var Manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.ManufacturerId == product.ManufacturerId);
            var Ram = await _context.Rams.FirstOrDefaultAsync(r => r.RamId == product.RamId);
            var Rom = await _context.Roms.FirstOrDefaultAsync(r => r.RomId == product.RomId);
            return Manufacturer.ManufacturerName + "_" + product.NameProduct + "_" + Ram.Memory + "_" + Rom.Space + Path.GetExtension(fileName);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(WareHouse warehouse, ProImgAndModelduct product, Rom rom, Ram ram)
        {
            if (product.NameProduct == null)
            {
                ModelState.AddModelError("NameProduct", "Vui lòng điền trường này");
            }
            string uniqueFileName = await GetUniqueFileName(product, product.ImgFile.FileName);
            //Thêm hình
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImg");
            var filePath = Path.Combine(uploads, uniqueFileName);
            product.ImgFile.CopyTo(new FileStream(filePath, FileMode.Create));

            Product productNew = new Product();
            productNew.NameProduct = product.NameProduct;
            productNew.ManufacturerId = product.ManufacturerId;
            productNew.RamId = product.RamId;
            productNew.RomId = product.RomId;
            productNew.ImgUrl = uniqueFileName;
            if (product.Discount != null)
            {
                productNew.DiscountId = product.DiscountId;
            }
            productNew.Note = product.Note;
            productNew.Price = product.Price;

            _context.Products.Add(productNew);
            await _context.SaveChangesAsync();
            DateTime createDate = DateTime.Now;

            WareHouse wareHouse = new WareHouse()
            {
                ProductId = productNew.ProductId,
                Cost = warehouse.Cost,
                QuantityImported = warehouse.QuantityImported,
                QuantitySold = 0,
                Date = createDate,
            };
            _context.WareHouses.Add(wareHouse);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
