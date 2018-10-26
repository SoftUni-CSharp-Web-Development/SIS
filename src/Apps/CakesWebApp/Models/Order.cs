using System;
using System.Collections.Generic;

namespace CakesWebApp.Models
{
    public class Order : BaseModel<int>
    {
        public Order()
        {
            this.Products = new HashSet<OrderProduct>();
        }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime DateOfCreation { get; set; } = DateTime.UtcNow;

        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}
