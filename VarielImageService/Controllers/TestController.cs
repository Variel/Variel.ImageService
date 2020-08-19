using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Variel.ImageService.Data;

namespace Variel.ImageService.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return File(await ImageProcessor.ResizeAsync(file.OpenReadStream(), new ProcessingPreset
            {
                Width = 120,
                Height = 120,
                PreserveAlpha = false,
                ResizeOption = ResizeOption.AspectFill,
                Quality = 1
            }),"image/jpeg", file.FileName);
        }
    }
}
