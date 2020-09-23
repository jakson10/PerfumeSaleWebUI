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
    public class OrderDetailApiManager : IOrderDetailApiService
    {
        private readonly HttpClient _httpClient;

        public OrderDetailApiManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/orderdetails/");
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            var jsonData = JsonConvert.SerializeObject(orderDetail);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("", content);

        }
    }
}
