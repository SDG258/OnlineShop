using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class InfoProduct
    {
        public IEnumerable<Manufacturer> ManufacturerViewModel { get; set; }
        public IEnumerable<Discount> DiscountViewModel { get; set; }
        public IEnumerable<Ram> RamViewModel { get; set; }
        public IEnumerable<Rom> RomViewModel { get; set; }
    }
}