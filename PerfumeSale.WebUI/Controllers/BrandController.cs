using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using PerfumeSale.WebUI.Models.EntitiesModel;
using Serilog;

namespace PerfumeSale.WebUI.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandApiService _brandApiService;

        public BrandController(IBrandApiService brandApiService)
        {
            _brandApiService = brandApiService;
        }
        public async Task<IActionResult> GetAll()
        {

            return View(await _brandApiService.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View(new Brand());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (await _brandApiService.GetByBrandNameAsync(brand.BrandName))
            {
                await _brandApiService.AddAsync(brand);
                ViewBag.ErrorMessage = "";
                return RedirectToAction("GetAll", "Brand");
            }
            ViewBag.ErrorMessage = "Sistemde aynı isme ait marka bulunmaktadır...!";
            return View(brand);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await _brandApiService.GetAsync(id));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _brandApiService.DeleteAsync(id);
            return RedirectToAction("GetAll", "Brand");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _brandApiService.GetAsync(id);
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand)
        {
            if (await _brandApiService.GetByBrandNameAsync(brand.BrandName))
            {
                await _brandApiService.UpdateAsync(brand);
                ViewBag.ErrorMessage = "";
                return RedirectToAction("GetAll", "Brand");
            }
            ViewBag.ErrorMessage = "Sistemde aynı isme ait marka bulunmaktadır...!";
            return View(brand);
        }

    }
}
