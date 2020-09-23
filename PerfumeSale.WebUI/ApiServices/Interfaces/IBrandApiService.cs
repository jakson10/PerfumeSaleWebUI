using PerfumeSale.WebUI.Models.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Interfaces
{
    public interface IBrandApiService
    {
        Task<List<Brand>> GetAllAsync();
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(int id);
        Task<Brand> GetAsync(int id);
        Task<bool> GetByBrandNameAsync(string brandName);

    }
}
