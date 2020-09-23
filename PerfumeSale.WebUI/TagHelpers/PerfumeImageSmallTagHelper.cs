using Microsoft.AspNetCore.Razor.TagHelpers;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.TagHelpers
{
    [HtmlTargetElement("getperfumeimagesmall")]
    public class PerfumeImageSmallTagHelper : TagHelper
    {
        private readonly IImageApiService _imageApiService;
        public PerfumeImageSmallTagHelper(IImageApiService imageApiService)
        {
            _imageApiService = imageApiService;
        }
        public int Id { get; set; }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var blob = await _imageApiService.GetPerfumeImagePathAsync(Id);
            string html = string.Empty;

            html = $"<img src='{blob}' style='width: 50px; height: 50px; border-radius:50%; border:1px solid #999;  object-fit:cover; object-position: 50% 1%;'/>";

            output.Content.SetHtmlContent(html);
        }
    }
}
