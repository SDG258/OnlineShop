using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class Comment
    {
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public string Cmt { get; set; }
        public int? Rate { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
