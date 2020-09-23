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
    public class OrderApiManager : IOrderApiService
    {
        private readonly HttpClient _httpClient;
        public OrderApiManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/orders/");
        }

        public async Task<Order> AddAsync(Order order)
        {
            var jsonData = JsonConvert.SerializeObject(order);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await _httpClient.PostAsync("", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Order>(await responseMessage.Content.ReadAsStringAsync());
            }
            return null;
        }

    }
}
