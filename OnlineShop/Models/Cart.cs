﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models
{
    public class Cart
    {
        public Product product { get; set; }
        public int Quantity { get; set; }
        public int Sum { get; set; }
        public float Total { get; set; }
    }
}