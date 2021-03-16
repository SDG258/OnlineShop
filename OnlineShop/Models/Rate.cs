using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Rate
    {
        public int? Level { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? CreatAt { get; set; }
        public int RateId { get; set; }
        public string Comments { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
