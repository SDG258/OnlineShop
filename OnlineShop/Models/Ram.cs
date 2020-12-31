using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Ram
    {
        public Ram()
        {
            Products = new HashSet<Product>();
        }

        public int RamId { get; set; }
        public int? Memory { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
