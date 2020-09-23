using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.Models.EntitiesModel
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int PerfumeId { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }


        public virtual Order Order { get; set; }
        public virtual Perfume Perfume { get; set; }

    }
}
