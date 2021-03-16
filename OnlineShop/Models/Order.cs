using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Order
    {
        public Order()
        {
            DetailOrders = new HashSet<DetailOrder>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public double? Total { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public string Note { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<DetailOrder> DetailOrders { get; set; }
    }
}
