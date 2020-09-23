using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.Models.EntitiesModel
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserDetailId { get; set; }
        public string ShipAddress { get; set; }
        public string Phone { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual UserDetail UserDetail { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
