using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using PerfumeSale.WebUI.Extensions;
using PerfumeSale.WebUI.Models;
using PerfumeSale.WebUI.Models.EntitiesModel;

namespace PerfumeSale.WebUI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderApiService _orderApiService;
        private readonly IOrderDetailApiService _orderDetailApiService;
        public OrderController(IOrderApiService orderApiService, IOrderDetailApiService orderDetailApiService)
        {
            _orderApiService = orderApiService;
            _orderDetailApiService = orderDetailApiService;

        }
        public IActionResult CompleteTheShopping()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CompleteTheShopping(Order model)
        {
            model.OrderDate = DateTime.Now;
            var userDetailId = HttpContext.Session.GetString("userDetailId");
            model.UserDetailId = int.Parse(userDetailId);
            var response = await _orderApiService.AddAsync(model);

            return RedirectToAction("DistributeOrders", response);
        }

        public async Task<IActionResult> DistributeOrders(Order response)
        {
            List<BasketModel> perfumes = HttpContext.Session.GetObject<List<BasketModel>>("basket");
            OrderDetail orderDetail;
            foreach (var item in perfumes)
            {
                orderDetail = new OrderDetail()
                {
                    OrderId = response.OrderId,
                    PerfumeId = item.PerfumeId,
                    Price = item.Price,
                    Count = item.Count
                };
                await _orderDetailApiService.AddAsync(orderDetail);
            }
            HttpContext.Session.Remove("basket");
            return RedirectToAction("ThankYouMessage", response);
        }
        public IActionResult ThankYouMessage()
        {
            return View();
        }

    }

}
