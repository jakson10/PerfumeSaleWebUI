using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using PerfumeSale.WebUI.Models;
using PerfumeSale.WebUI.Models.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Concrete
{
    public class AuthApiManager : IAuthApiService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        public AuthApiManager(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/userdetails/");
        }
        public async Task<UserDetail> SignIn(LoginModel model)
        {
            var responseMessage = await _httpClient.GetAsync($"SignIn/{model.FirstName}/{model.LastName}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return await ResponseMessageSolvent(responseMessage);
            }
            return await AddAsync(model);
        }

        public async Task<UserDetail> AddAsync(LoginModel model)
        {
            var userDetail = new UserDetail()
            {
                UserName = model.FirstName,
                FirstName = model.LastName,
                LastName = "admin",
                Email = "admin@admin.com",
                Phone = "5555555555",
                Address = "İstanbul-Beykoz"
            };

            var jsonData = JsonConvert.SerializeObject(userDetail);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await _httpClient.PostAsync("", content);

            if (responseMessage.IsSuccessStatusCode)
            {
                return await ResponseMessageSolvent(responseMessage);
            }
            return null;
        }

        private async Task<UserDetail> ResponseMessageSolvent(HttpResponseMessage responseMessage)
        {
            var user = JsonConvert.DeserializeObject<UserDetail>(await responseMessage.Content.ReadAsStringAsync());
            _httpContextAccessor.HttpContext.Session.SetString("userDetailId", user.UserDetailId.ToString());
            _httpContextAccessor.HttpContext.Session.SetString("userName", user.UserName);
            return user;
        }

    }
}
