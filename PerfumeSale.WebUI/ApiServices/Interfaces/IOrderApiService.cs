using PerfumeSale.WebUI.Models.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Interfaces
{
    public interface IOrderApiService
    {
        Task<Order> AddAsync(Order order);
    }
}
