using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExceptionHandler.Filters
{
    public class CustomExceptionFilter : Attribute, IExceptionFilter
    {
        private ILogger<CustomExceptionFilter> _logger;
        private IWebHostEnvironment _hostingEnvironment;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger,IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            //如果异常未处理
            if(!context.ExceptionHandled)
            {
                //如果是开发环境
                if(_hostingEnvironment.EnvironmentName == "Development")
                {
                    Console.WriteLine($"异常捕获：{context.Exception.Message}");
                    //var result = new ViewResult { ViewName = "../Handle/Index" };
                    ////result.ViewData = new ViewDataDictionary(_modelMetadataProvider,
                    ////                                            context.ModelState);
                    //result.ViewData.Add("Exception", context.Exception);//传递数据
                    //context.Result = result;
                }
                else
                {
                    context.Result = new JsonResult(new
                    {
                        Code = 500,
                        Message = context.Exception.Message
                    });
                }
            }
            context.ExceptionHandled = true;
        }
    }
}
