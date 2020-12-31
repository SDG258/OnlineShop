using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Product
    {
        public Product()
        {
            WareHouses = new HashSet<WareHouse>();
        }

        public int ProductId { get; set; }
        public string NameProduct { get; set; }
        public string ImgUrl { get; set; }
        public int? Price { get; set; }
        public int? DiscountId { get; set; }
        public int RomId { get; set; }
        public int RamId { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
        public int ManufacturerId { get; set; }

        public virtual Discount Discount { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual Ram Ram { get; set; }
        public virtual Rom Rom { get; set; }
        public virtual ICollection<WareHouse> WareHouses { get; set; }
    }
}
