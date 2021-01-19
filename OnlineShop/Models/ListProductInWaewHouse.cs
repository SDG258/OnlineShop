using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class ListProductInWaewHouse
    {
        public IEnumerable<WareHouse> WareHouseiewModel { get; set; }
        public IEnumerable<Product> ProductViewModel { get; set; }
    }
}
