using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.Models
{
    public class BasketModel
    {
        public int PerfumeId { get; set; }
        public string PerfumeName { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
}
