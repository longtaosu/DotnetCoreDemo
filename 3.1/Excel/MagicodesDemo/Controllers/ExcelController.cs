using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using Magicodes.ExporterAndImporter.Core;
//using Magicodes.ExporterAndImporter.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Magicodes;
using Magicodes.ExporterAndImporter.Excel;
using Magicodes.ExporterAndImporter.Core;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace MagicodesDemo.Controllers
{
    /// <summary>
    /// https://github.com/dotnetcore/Magicodes.IE/blob/master/docs/2.%E5%9F%BA%E7%A1%80%E6%95%99%E7%A8%8B%E4%B9%8B%E5%AF%BC%E5%87%BAExcel.md
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private ILogger _logger;
        public ExcelController(ILogger<ExcelController> logger)
        {
            _logger = logger;
        }

        public IActionResult TestLog()
        {
            _logger.LogInformation("information级错误");
            _logger.LogError("error级错误");

            return Ok("日志");
        }

        public IActionResult Test()
        {
            return Ok("test");
        }


        /// <summary>
        /// 简单数据导出
        /// </summary>
        /// <returns></returns>
        public IActionResult ExportSimple()
        {
            IExporter exporter = new ExcelExporter();

            var data = GetData();

            //web文件下载
            var file = exporter.ExportAsByteArray(data).Result;
            return File(file, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + ".xlsx");

            //导出到本地
            //await exporter.Export("a.xlsx", data);
        }

        /// <summary>
        /// 简单模板导出
        /// </summary>
        /// <returns></returns>
        public IActionResult ExportByTemplate()
        {
            IExportFileByTemplate exporter = new ExcelExporter();

            var data = GetData();
            var result = data.Obj2ExcelResult<Student>();

            //web文件下载
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Template.xlsx");
            var file = exporter.ExportBytesByTemplate(result, path).Result;
            return File(file, "application/vnd.ms-excel", $"文件下载_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");

            //导出到本地
            //await exporter.ExportByTemplate("b.xlsx", data,Directory.GetCurrentDirectory()+@"\Templates\template.xlsx");
        }

        /// <summary>
        /// 复杂模板导出
        /// </summary>
        /// <returns></returns>
        public IActionResult ExportByComplex_Web()
        {
            try
            {
                Console.WriteLine("test");

                _logger.LogInformation("test");
                _logger.LogError("test");

                IExportFileByTemplate exporter = new ExcelExporter();

                var summary = new Summary()
                {
                    Author = "lts",
                    Time = DateTime.Now.ToString(),
                    Data = GetData()
                };

                //浏览器下载
                var fileType = "application/octet-stream";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Template.xlsx");
                var file = exporter.ExportBytesByTemplate(summary, path).Result;
                return File(file, fileType, "a文件下载.xlsx");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                Console.WriteLine(ex);
                return Ok("发生异常");
            }
        }

        public IActionResult DownloadLocalFile(string fileName)
        {
            var memory = new MemoryStream();
            var path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            var type = "application/octet-stream";
            return File(memory, type, "b文件下载.xlsx");

        }

        public async Task ExportByComplex_Local()
        {
            IExportFileByTemplate exporter = new ExcelExporter();

            var summary = new Summary()
            {
                Author = "lts",
                Time = DateTime.Now.ToString(),
                Data = GetData()
            };

            //输出到本地文件
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Complex.xlsx");
            await exporter.ExportByTemplate("a本地中文名测试.xlsx", summary, path);
        }


        public IActionResult ImportData()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory() ,"c.xlsx");

            IExcelImporter importer = new ExcelImporter();
            var import = importer.Import<StudentImport>(filePath).Result;
            var data = import.Data;

            return Ok();
        }

        private List<Student> GetData()
        {
            List<Student> data = new List<Student>();

            data.Add(new Student()
            {
                Name = "a",
                Age = 18
            });
            data.Add(new Student()
            {
                Name = "b",
                Age = 19
            });
            data.Add(new Student()
            {
                Name = "c",
                Age = 20
            });

            return data;
        }
    }

    [ExcelExporter(Name ="学生信息",TableStyle ="Light10",AutoFitAllColumn =true)]
    public class Student
    {
        //[ExporterHeader(DisplayName ="姓名")]
        public string Name { get; set; }
        //[ExporterHeader(DisplayName ="年龄",Format ="#,##0")]
        public int Age { get; set; }
    }

    [ExcelImporter(IsLabelingError = true)]
    public class StudentImport
    {
        [MaxLength(3)]
        [ImporterHeader(Name = "姓名")]
        public string Name { get; set; }
        [ImporterHeader(Name ="年龄")]
        public int Age { get; set; }
    }


    public class Summary
    {
        public string Author { get; set; }

        public string Time { get; set; }

        public List<Student> Data { get; set; }
    }

    public class ExcelResult<T>
    {
        public List<T> Data { get; set; }
    }

    public static class ExcelResultExtension
    {
        public static ExcelResult<T> Obj2ExcelResult<T>(this List<T> data)
        {
            ExcelResult<T> result = new ExcelResult<T>();
            result.Data = data;
            return result;
        }

    }
}
