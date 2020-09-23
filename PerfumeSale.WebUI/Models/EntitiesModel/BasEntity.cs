using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.Models.EntitiesModel
{
    public class BasEntity
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedUsername { get; set; }

        public string PerfumeName { get; set; }

        public int PerfumeCount { get; set; }

        public DateTime ModifierDate { get; set; }

        public string ModifierUsername { get; set; }

        public DateTime DeletedDate { get; set; }

        public string DeletedUsername { get; set; }

    }
}
