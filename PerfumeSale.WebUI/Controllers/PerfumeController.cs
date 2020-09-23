using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using PerfumeSale.WebUI.Extensions;
using PerfumeSale.WebUI.Models;
using PerfumeSale.WebUI.Models.EntitiesModel;

namespace PerfumeSale.WebUI.Controllers
{
    public class PerfumeController : Controller
    {
        private readonly IPerfumeApiService _perfumeApiService;
        private readonly IBrandApiService _brandApiService;
        private readonly ILogger<PerfumeController> _logger;
        public PerfumeController(IPerfumeApiService perfumeApiService, IBrandApiService brandApiService, ILogger<PerfumeController> logger)
        {
            _perfumeApiService = perfumeApiService;
            _brandApiService = brandApiService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<JsonResult> GetAllBrands()
        {
            var brands = await _brandApiService.GetAllAsync();
            var brandList = new SelectList(brands, "BrandId", "BrandName");
            return Json(brandList);
        }

        public IActionResult Create()
        {
            return View(new PerfumeAddModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(PerfumeAddModel model)
        {
            //string girilince value 0 alıyor
            if (model.Price == 0)
            {
                return View(model);
            }
            if (await _perfumeApiService.GetByPerfumeNameAsync(model.PerfumeName))
            {
                if (model.Image != null)
                {
                    var newName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/" + newName);
                    using (var stream = System.IO.File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                    await _perfumeApiService.AddAsync(model, newName);
                }
                else
                {
                    await _perfumeApiService.AddAsync(model, null);
                }
                ViewBag.ErrorMessage = "";
                return RedirectToAction("GetAll", "Perfume");
            }

            ViewBag.ErrorMessage = "Sistemde aynı isme ait parfüm bulunmaktadır...!";
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var perfume = await _perfumeApiService.GetAsync(id);
            return View(new PerfumeUpdateModel
            {
                PerfumeId = perfume.PerfumeId,
                PerfumeName = perfume.PerfumeName,
                Price = perfume.Price,
                BrandId = perfume.BrandId
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PerfumeUpdateModel model)
        {
            //string girilince value 0 alıyor
            if (model.Price == 0)
            {
                return View(model);
            }
            if (await _perfumeApiService.GetByPerfumeNameAsync(model.PerfumeName))
            {
                if (model.Image != null)
                {
                    var newName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/" + newName);
                    using (var stream = System.IO.File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                    await _perfumeApiService.UpdateAsync(model, newName);
                }
                else
                {
                    await _perfumeApiService.UpdateAsync(model, null);
                }
                ViewBag.ErrorMessage = "";
                return RedirectToAction("GetAll", "Perfume");
            }
            ViewBag.ErrorMessage = "Sistemde aynı isme ait parfüm bulunmaktadır...!";
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _perfumeApiService.DeleteAsync(id);
            return RedirectToAction("GetAll", "Perfume");
        }

        public async Task<IActionResult> AddToBasket(int id)
        {
            var logEntity = new BasEntity();
            List<BasketModel> perfumes = HttpContext.Session.GetObject<List<BasketModel>>("basket");
            var userName = HttpContext.Session.GetString("userName");
            var perfumeToAdd = await _perfumeApiService.GetAsync(id);
            if (perfumes == null)
            {
                perfumes = new List<BasketModel>();
            }

            var result = (from item in perfumes
                          where item.PerfumeId == perfumeToAdd.PerfumeId
                          select item).FirstOrDefault();

            if (result == null)
            {
                BasketModel model = new BasketModel
                {
                    PerfumeId = perfumeToAdd.PerfumeId,
                    PerfumeName = perfumeToAdd.PerfumeName,
                    Price = perfumeToAdd.Price,
                    Count = 1
                };
                perfumes.Add(model);
                logEntity.PerfumeName = model.PerfumeName;
                logEntity.PerfumeCount = model.Count;
            }
            else
            {
                result.Count++;
                logEntity.PerfumeName = result.PerfumeName;
                logEntity.PerfumeCount = result.Count;
            }

            AddToCartPrintLogMessage(logEntity.PerfumeName, logEntity.PerfumeCount, userName);

            HttpContext.Session.SetObject("basket", perfumes);

            return RedirectToAction("GetAll", "Perfume");
        }

        public IActionResult ViewCart(int decrease, int remove)
        {
            List<BasketModel> perfumes = HttpContext.Session.GetObject<List<BasketModel>>("basket");
            var userName = HttpContext.Session.GetString("userName");
            if (perfumes == null)
            {
                perfumes = new List<BasketModel>();
                return View(perfumes);
            }
            else if (decrease > 0 && remove == 0)
            {
                var result = (from item in perfumes
                              where item.PerfumeId == decrease
                              select item).FirstOrDefault();
                result.Count--;

                if (result.Count == 0)
                {
                    perfumes.RemoveAll(x => x.PerfumeId == decrease);
                }
                ReducefromCartPrintLogMessage(result.PerfumeName, result.Count, userName);
            }
            else if (decrease == 0 && remove > 0)
            {
                var entity = perfumes.Find(x => x.PerfumeId == remove);
                perfumes.RemoveAll(x => x.PerfumeId == remove);

                OutOfCartPrintLogMessage(entity.PerfumeName, userName);

            }
            HttpContext.Session.Remove("basket");
            HttpContext.Session.SetObject("basket", perfumes);
            return View(perfumes);
        }

        public async Task<IActionResult> GetAll(int brandId, string s, int price, int activePage = 1)
        {
            List<Perfume> perfumes = null;
            ViewBag.SearchedWord = s;
            ViewBag.ActivePage = activePage;
            ViewBag.Brands = await _brandApiService.GetAllAsync();
            int tmpTotalPage = 0;

            if (brandId <= 0 && s == null && price <= 0)
            {
                perfumes = CalculateListLength(out int totalPage, await _perfumeApiService.GetPerfumeByActivePageAsync(activePage));
                tmpTotalPage = totalPage;
                ViewBag.TotalPage = tmpTotalPage;
                TempData["Price"] = 0;
                TempData["BrandId"] = 0;
            }
            else if (s != null && brandId <= 0 && price <= 0)
            {
                perfumes = await _perfumeApiService.GetPerfumeByParfumeNameAsync(s);
                ViewBag.TotalPage = perfumes.Count();
                TempData["Price"] = 0;
                TempData["BrandId"] = 0;
            }
            else if (s == null && price <= 0 && brandId > 0)
            {
                perfumes = CalculateListLength(out int totalPage, await _perfumeApiService.GetPerfumeByBrandIdAsync(brandId, activePage));
                tmpTotalPage = totalPage;
                ViewBag.TotalPage = tmpTotalPage;
                TempData["Price"] = 0;
                TempData["BrandId"] = brandId;
            }

            else if (price == 1 && brandId > 0)
            {
                perfumes = CalculateListLength(out int totalPage, await _perfumeApiService.GetSortPerfumesInAscByBrandIdAsync(brandId, activePage));
                tmpTotalPage = totalPage;
                ViewBag.TotalPage = tmpTotalPage;
                TempData["Price"] = price;
                TempData["BrandId"] = brandId;
            }

            else if (price == 2 && brandId > 0)
            {
                perfumes = CalculateListLength(out int totalPage, await _perfumeApiService.GetSortPerfumesInDescByBrandIdAsync(brandId, activePage));
                tmpTotalPage = totalPage;
                ViewBag.TotalPage = tmpTotalPage;
                TempData["Price"] = price;
                TempData["BrandId"] = brandId;
            }
            else
            {
                if (price == 1)
                {
                    perfumes = CalculateListLength(out int totalPage, await _perfumeApiService.GetPerfumeBySortBySalarAscyAsync(activePage));
                    tmpTotalPage = totalPage;
                    ViewBag.TotalPage = tmpTotalPage;
                    TempData["Price"] = price;
                    TempData["BrandId"] = 0;
                }
                else if (price == 2)
                {
                    perfumes = CalculateListLength(out int totalPage, await _perfumeApiService.GetPerfumeBySortBySalarDescyAsync(activePage));
                    tmpTotalPage = totalPage;
                    ViewBag.TotalPage = tmpTotalPage;
                    TempData["Price"] = price;
                    TempData["BrandId"] = 0;
                }

            }
            return View(perfumes);
        }

        private List<Perfume> CalculateListLength(out int totalPage, List<Perfume> perfumes)
        {
            var elementWithLength = (from perfume in perfumes
                                     where perfume.PerfumeName == "Liste Uzunlugu"
                                     && perfume.Price == 1907
                                     select perfume).FirstOrDefault();
            totalPage = elementWithLength.PerfumeId;
            perfumes.Remove(elementWithLength);
            return perfumes;
        }


        private void AddToCartPrintLogMessage(string perfumeName, int count, string userName = "isimsiz")
        {
            _logger.LogWarning($" '{DateTime.Now}' vaktinde, '{userName}' adlı kullanıcı, '{perfumeName}' isimli ürünü, 1 adet kadar sepetine eklemiştir ve sepetindeki toplam '{perfumeName}' sayisi '{count}' adettir..");
        }

        private void ReducefromCartPrintLogMessage(string perfumeName, int count, string userName = "isimsiz")
        {
            _logger.LogWarning($" '{DateTime.Now}' vaktinde, '{userName}' adlı kullanıcı, '{perfumeName}' isimli ürünü, 1 adet kadar sepetinden çıkarmıştır ve sepetindeki toplam '{perfumeName}' sayisi '{count}' adettir..");
        }

        private void OutOfCartPrintLogMessage(string perfumeName, string userName = "isimsiz")
        {
            _logger.LogWarning($" '{DateTime.Now}' vaktinde, '{userName}' adlı kullanıcı, '{perfumeName}' isimli ürünü sepetinden kaldırmıştır");
        }
    }

}
