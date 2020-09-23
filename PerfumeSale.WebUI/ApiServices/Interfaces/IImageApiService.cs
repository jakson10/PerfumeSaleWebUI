using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Interfaces
{
    public interface IImageApiService
    {
        Task<string> GetPerfumeImagePathAsync(int id);
        
    }
}
