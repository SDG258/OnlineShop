using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class DetailOrder
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? DataCreate { get; set; }
        public double? Price { get; set; }
        public int? Amount { get; set; }

        public virtual Order Order { get; set; }
    }
}
