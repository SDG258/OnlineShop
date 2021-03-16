using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class DiscountManufacturer
    {
        public IEnumerable<Discount> DiscountModel { get; set; }
        public IEnumerable<Manufacturer> ManufacturerModel { get; set; }
    }
}
