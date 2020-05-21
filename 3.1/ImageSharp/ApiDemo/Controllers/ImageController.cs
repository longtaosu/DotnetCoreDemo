using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private ICaptchaService _captchaService;
        public ImageController(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        static int count = 0;
        [HttpGet]
        public IActionResult GetCode()
        {
            //var (code, bytes) = CaptchaHelper.GenVCode(4);

            //// code handle logic
            //System.Console.WriteLine(code);
            //Console.WriteLine(++count);

            //return File(bytes, "image/png");

            //_captchaService.SetOptions(new CaptchaOptions()
            //{
            //    Height = 35,
            //    Width = 90
            //});

            var text = _captchaService.GetRandomText(5);
            var bytes = _captchaService.GetCaptcha(text);
            Console.WriteLine(text);
            Console.WriteLine(++count);
            return File(bytes, "image/png");
        }
    }
}