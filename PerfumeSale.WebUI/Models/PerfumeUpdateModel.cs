﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.Models
{
    public class PerfumeUpdateModel
    {
        public int PerfumeId { get; set; }
        public string PerfumeName { get; set; }
        public int BrandId { get; set; }
        public double Price { get; set; }

        public IFormFile Image { get; set; }

    }
}
