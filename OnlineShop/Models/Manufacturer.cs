﻿using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Manufacturer
    {
        public Manufacturer()
        {
            Discounts = new HashSet<Discount>();
            Products = new HashSet<Product>();
        }

        public int ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }

        public virtual ICollection<Discount> Discounts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
