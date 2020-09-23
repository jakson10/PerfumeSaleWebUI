using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.Models.EntitiesModel
{
    public class Perfume
    {
        public int PerfumeId { get; set; }
        public string PerfumeName { get; set; }
        public int BrandId { get; set; }
        public double Price { get; set; }
        public string PhotoPath { get; set; }

        public virtual Brand Brand { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
