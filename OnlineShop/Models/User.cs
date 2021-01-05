using System;
using System.Collections.Generic;

#nullable disable

namespace OnlineShop.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Orders = new HashSet<Order>();
            Rates = new HashSet<Rate>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public int? Permission { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
    }
}
