using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeihanLi.Npoi;
using WeihanLi.Npoi.Attributes;

namespace WeihanliDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        /// <summary>
        /// 直接导出，列头可以指定
        /// </summary>
        [HttpGet]
        public IActionResult TestExport()
        {
            var data = GetData();

            //输出到本地
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "exportTest.xlsx");
            //data.ToExcelFile(path);
            //return Ok("本地文件生成完成");

            //web下载
            var file = data.ToExcelBytes(ExcelFormat.Xlsx);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Template.xlsx");
            return File(file, "application/vnd.ms-excel", "文件下载.xlsx");
        }

        public IActionResult TestExportByTemplate()
        {
            var data = GetData();

            //输出到本地
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "Tempaltes", "Template.xlsx");
            //data.ToExcelFileByTemplate(
            //    path,
            //    Directory.GetCurrentDirectory() + "test.xlsx",
            //    extraData: new
            //    {
            //        Author = "lts",
            //        Title = "Template export test"
            //    }
            //);
            //return Ok("本地文件生成完成");

            //web下载
            var template = Path.Combine(Directory.GetCurrentDirectory(), "Tempaltes", "Template.xlsx");
            var file = data.ToExcelBytesByTemplate(template, extraData: new
            {
                Author = "lts",
                Title = "Template export test"
            });
            return File(file, "application/vnd.ms-excel", "文件下载.xlsx");
        }

        /// <summary>
        /// 测试数据导入，可以直接映射字段
        /// </summary>
        [HttpGet]
        public void TestImport()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "exportTest.xlsx");

            var data = ExcelHelper.ToEntityList<Test>(path);
        }



        private List<Test> GetData()
        {
            var entities = new List<Test>()
            {
                new Test()
                {
                     Age = DateTime.Now.Millisecond,
                     Name = "张三",
                     Sex = true
                },
                new Test()
                {
                     Age = DateTime.Now.Millisecond,
                     Name = "李四",
                     Sex = false
                }
            };

            return entities;
        }
    }


    public class Test
    {
        //[Column(Title = "姓名",Index =3)]
        [Column(Title = "姓名")]
        public string Name { get; set; }
        //[Column(Title ="年龄",Index = 2)]
        [Column(Title = "年龄")]
        public int Age { get; set; }
        //[Column(Title = "性别",Index = 1)]
        [Column(Title = "性别")]
        public bool Sex { get; set; }
    }
}
