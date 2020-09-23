using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using PerfumeSale.WebUI.Models.EntitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Concrete
{
    public class BrandApiManager : IBrandApiService
    {
        private readonly HttpClient _httpClient;

        public BrandApiManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/brands");
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            var responseMesasage = await _httpClient.GetAsync("");
            if (responseMesasage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<Brand>>(await responseMesasage.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task AddAsync(Brand brand)
        {
            var jsonData = JsonConvert.SerializeObject(brand);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("", content);
        }

        public async Task UpdateAsync(Brand brand)
        {
            var jsonData = JsonConvert.SerializeObject(brand);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            await _httpClient.PutAsync($"{brand.BrandId}", content);
        }

        public async Task DeleteAsync(int id)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/brands/");
            await _httpClient.DeleteAsync($"{id}");
        }

        public async Task<Brand> GetAsync(int id)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/brands/");
            var responseMessage = await _httpClient.GetAsync($"{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Brand>(await responseMessage.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<bool> GetByBrandNameAsync(string brandName)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/brands/");
            var responseMessage = await _httpClient.GetAsync($"GetBrandByBrandName/{brandName}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
