using PerfumeSale.WebUI.Models;
using PerfumeSale.WebUI.Models.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Interfaces
{
    public interface IAuthApiService
    {
        Task<UserDetail> SignIn(LoginModel model);
        Task<UserDetail> AddAsync(LoginModel model);
    }
}
