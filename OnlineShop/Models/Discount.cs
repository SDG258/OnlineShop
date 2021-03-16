using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Products = new HashSet<Product>();
        }

        public int DiscountId { get; set; }
        public double? DiscountPercent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Used { get; set; }
        public int? AmountOf { get; set; }
        public int? ManufacturerId { get; set; }
        public string Note { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
