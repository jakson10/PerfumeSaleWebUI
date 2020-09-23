using Microsoft.AspNetCore.Razor.TagHelpers;
using PerfumeSale.WebUI.ApiServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfumeSale.WebUI.TagHelpers
{

    [HtmlTargetElement("getperfumeimage")]
    public class PerfumeImageTagHelper : TagHelper
    {
        private readonly IImageApiService _imageApiService;
        public PerfumeImageTagHelper(IImageApiService imageApiService)
        {
            _imageApiService = imageApiService;
        }
        public int Id { get; set; }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var blob = await _imageApiService.GetPerfumeImagePathAsync(Id);
            string html = string.Empty;

            html = $"<img src='{blob}' class='card-img-top'/>";

            output.Content.SetHtmlContent(html);
        }
    }
}
