using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using PerfumeSale.WebUI.Models;
using PerfumeSale.WebUI.Models.EntitiesModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.ApiServices.Concrete
{
    public class PerfumeApiManager : IPerfumeApiService
    {
        private readonly HttpClient _httpClient;
        public PerfumeApiManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/perfumes");
        }
        public async Task AddAsync(PerfumeAddModel model, string newName)
        {
            MultipartFormDataContent formData = new MultipartFormDataContent();

            if (model.Image != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/" + newName);
                var bytes = await System.IO.File.ReadAllBytesAsync(path);
                ByteArrayContent byteArrayContent = new ByteArrayContent(bytes);
                byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.Image.ContentType);

                formData.Add(byteArrayContent, nameof(PerfumeAddModel.Image), model.Image.FileName);
            }
            formData.Add(new StringContent(model.PerfumeName), nameof(PerfumeAddModel.PerfumeName));
            formData.Add(new StringContent(model.BrandId.ToString()), nameof(PerfumeAddModel.BrandId));
            formData.Add(new StringContent(model.Price.ToString()), nameof(PerfumeAddModel.Price));

            await _httpClient.PostAsync("", formData);
        }

        public async Task UpdateAsync(PerfumeUpdateModel model, string newName)
        {

            MultipartFormDataContent formData = new MultipartFormDataContent();
            if (model.Image != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/" + newName);
                var bytes = await System.IO.File.ReadAllBytesAsync(path);
                ByteArrayContent byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.Image.ContentType);

                formData.Add(byteContent, nameof(PerfumeUpdateModel.Image), model.Image.FileName);
            }
            formData.Add(new StringContent(model.PerfumeId.ToString()), nameof(PerfumeUpdateModel.PerfumeId));
            formData.Add(new StringContent(model.PerfumeName), nameof(PerfumeUpdateModel.PerfumeName));
            formData.Add(new StringContent(model.BrandId.ToString()), nameof(PerfumeUpdateModel.BrandId));
            formData.Add(new StringContent(model.Price.ToString()), nameof(PerfumeUpdateModel.Price));

            await _httpClient.PutAsync($"{model.PerfumeId}", formData);
        }
        public async Task<bool> GetByPerfumeNameAsync(string perfumeName)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/perfumes/");
            var responseMessage = await _httpClient.GetAsync($"GetPerfumeByPerfumeName/{perfumeName}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Perfume>> GetAllAsync()
        {
            //http://localhost:56222/api/perfumes?$expand=brand
            var responseMesasage = await _httpClient.GetAsync($"?$expand=brand");
            if (responseMesasage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task<List<Perfume>> GetPerfumeByBrandIdAsync(int brandId, int acvitePage = 1)
        {
            //http://localhost:56222/api/perfumes?$filter=brandId eq 18 &$skip=1 &$top=1
            var responseMesasage = await _httpClient.GetAsync($"?$filter=brandId eq {brandId} &$expand=brand &$skip={((acvitePage - 1) * 8)} &$top={8}");
            if (responseMesasage.IsSuccessStatusCode)
            {
                var perfumes = JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());
                int count = 0;
                var responseCount = await _httpClient.GetAsync($"?$filter=brandId eq {brandId} &$expand=brand");
                {
                    if (responseCount.IsSuccessStatusCode)
                    {
                        count = JsonConvert.DeserializeObject<List<Perfume>>(await responseCount.Content.ReadAsStringAsync()).Count();
                    }
                }
                return SetListCount(perfumes, count);
            }
            return null;
        }

        public async Task<List<Perfume>> GetPerfumeByParfumeNameAsync(string parfumeName)
        {
            //http://localhost:56222/api/perfumes?$filter=perfumeName eq 'Parfüm1'

            var responseMesasage = await _httpClient.GetAsync($"?$filter=perfumeName eq '{parfumeName}' &$expand=brand");
            if (responseMesasage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());

            }
            return null;

        }

        public async Task<List<Perfume>> GetPerfumeByActivePageAsync(int activePage = 1)
        {
            //http://localhost:56222/api/perfumes?$filter=brandId eq 18
            var responseMesasage = await _httpClient.GetAsync($"?$expand=brand &$skip={((activePage - 1) * 8)} &$top={8}");
            if (responseMesasage.IsSuccessStatusCode)
            {
                var perfumes = JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());
                int count = 0;
                var responseCount = await _httpClient.GetAsync($"?$expand=brand");
                {
                    if (responseCount.IsSuccessStatusCode)
                    {
                        count = JsonConvert.DeserializeObject<List<Perfume>>(await responseCount.Content.ReadAsStringAsync()).Count();
                    }
                }
                return SetListCount(perfumes, count);
            }
            return null;
        }
        public async Task<List<Perfume>> GetPerfumeBySortBySalarAscyAsync(int acvitePage = 1)
        {
            //Küçükten büyüğe sıralar.
            var responseMesasage = await _httpClient.GetAsync($"?$orderby=price asc &$expand=brand &$skip={((acvitePage - 1) * 8)} &$top={8}");
            if (responseMesasage.IsSuccessStatusCode)
            {
                var perfumes = JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());
                int count = 0;
                var responseCount = await _httpClient.GetAsync($"?$orderby=price asc &$expand=brand");
                {
                    if (responseCount.IsSuccessStatusCode)
                    {
                        count = JsonConvert.DeserializeObject<List<Perfume>>(await responseCount.Content.ReadAsStringAsync()).Count();
                    }
                }
                return SetListCount(perfumes, count);
            }
            return null;
        }
        public async Task<List<Perfume>> GetPerfumeBySortBySalarDescyAsync(int acvitePage = 1)
        {
            //Büyükten küçüğe sıralar.
            var responseMesasage = await _httpClient.GetAsync($"?$orderby=price desc &$expand=brand &$skip={((acvitePage - 1) * 8)} &$top={8}");
            if (responseMesasage.IsSuccessStatusCode)
            {
                var perfumes = JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());
                int count = 0;
                var responseCount = await _httpClient.GetAsync($"?$orderby=price desc &$expand=brand");
                {
                    if (responseCount.IsSuccessStatusCode)
                    {
                        count = JsonConvert.DeserializeObject<List<Perfume>>(await responseCount.Content.ReadAsStringAsync()).Count();
                    }
                }
                return SetListCount(perfumes, count);
            }
            return null;

        }


        public async Task<List<Perfume>> GetSortPerfumesInAscByBrandIdAsync(int brandId, int acvitePage = 1)
        {
            //Küçükten büyüğe sıralar  markasına göre
            //http://localhost:56222/api/perfumes?$expand=brand?$expand=brand &$filter=brandId eq 25 &$orderby=price asc &$skip=1 &$top=1
            var responseMesasage = await _httpClient.GetAsync($"?$expand=brand &$filter=brandId eq {brandId} &$orderby=price asc &$skip={((acvitePage - 1) * 8)} &$top={8}");
            if (responseMesasage.IsSuccessStatusCode)
            {
                var perfumes = JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());
                int count = 0;
                var responseCount = await _httpClient.GetAsync($"?$expand=brand &$filter=brandId eq {brandId} &$orderby=price asc");
                {
                    if (responseCount.IsSuccessStatusCode)
                    {
                        count = JsonConvert.DeserializeObject<List<Perfume>>(await responseCount.Content.ReadAsStringAsync()).Count();
                    }
                }
                return SetListCount(perfumes, count);
            }
            return null;
        }

        public async Task<List<Perfume>> GetSortPerfumesInDescByBrandIdAsync(int brandId, int acvitePage = 1)
        {
            //Büyükten küçüğe sıralar  markasına göre
            //http://localhost:56222/api/perfumes?$expand=brand?$expand=brand &$filter=brandId eq 25 &$orderby=price desc &$skip=1 &$top=1
            var responseMesasage = await _httpClient.GetAsync($"?$expand=brand &$filter=brandId eq {brandId} &$orderby=price desc &$skip={((acvitePage - 1) * 8)} &$top={8}");
            if (responseMesasage.IsSuccessStatusCode)
            {
                var perfumes = JsonConvert.DeserializeObject<List<Perfume>>(await responseMesasage.Content.ReadAsStringAsync());
                int count = 0;
                var responseCount = await _httpClient.GetAsync($"?$expand=brand &$filter=brandId eq {brandId} &$orderby=price desc");
                {
                    if (responseCount.IsSuccessStatusCode)
                    {
                        count = JsonConvert.DeserializeObject<List<Perfume>>(await responseCount.Content.ReadAsStringAsync()).Count();
                    }
                }
                return SetListCount(perfumes, count);
            }
            return null;
        }

        public async Task<Perfume> GetAsync(int id)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/perfumes/");
            var responseMessage = await _httpClient.GetAsync($"{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Perfume>(await responseMessage.Content.ReadAsStringAsync());
            }
            return null;
        }

        public async Task DeleteAsync(int id)
        {
            _httpClient.BaseAddress = new Uri("http://localhost:56222/api/perfumes/");
            await _httpClient.DeleteAsync($"{id}");
        }

        private List<Perfume> SetListCount(List<Perfume> perfumes, int count)
        {
            var perfume = new Perfume()
            {
                PerfumeId = (int)Math.Ceiling((double)count / 8),
                PerfumeName = "Liste Uzunlugu",
                Price = 1907
            };
            perfumes.Add(perfume);
            return perfumes;
        }
    }
}
