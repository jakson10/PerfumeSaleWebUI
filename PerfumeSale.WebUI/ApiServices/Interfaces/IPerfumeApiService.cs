using PerfumeSale.WebUI.Models;
using PerfumeSale.WebUI.Models.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Interfaces
{
    public interface IPerfumeApiService
    {
        Task AddAsync(PerfumeAddModel model, string newName);
        Task UpdateAsync(PerfumeUpdateModel model, string newName);
        Task<List<Perfume>> GetAllAsync();
        Task<Perfume> GetAsync(int id);
        Task<List<Perfume>> GetPerfumeByBrandIdAsync(int brandId, int acvitePage);

        Task<List<Perfume>> GetPerfumeByParfumeNameAsync(string parfumeName);

        Task<List<Perfume>> GetPerfumeBySortBySalarAscyAsync(int activePage);
        Task<List<Perfume>> GetPerfumeBySortBySalarDescyAsync(int activePage);
        Task DeleteAsync(int id);
        Task<List<Perfume>> GetPerfumeByActivePageAsync(int activePage);
        Task<bool> GetByPerfumeNameAsync(string perfumeName);

        Task<List<Perfume>> GetSortPerfumesInAscByBrandIdAsync(int brandId, int acvitePage);
        Task<List<Perfume>> GetSortPerfumesInDescByBrandIdAsync(int brandId, int acvitePage);
    }
}
