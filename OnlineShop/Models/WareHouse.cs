using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class WareHouse
    {
        public int WareHouseId { get; set; }
        public int ProductId { get; set; }
        public int? Cost { get; set; }
        public int? QuantityImported { get; set; }
        public int? QuantitySold { get; set; }
        public DateTime? Date { get; set; }

        public virtual Product Product { get; set; }
    }
}
