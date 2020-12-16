using ElasticModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nest;
using SearchPart.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SearchPart.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ElasticSetting _elasticSetting;
        private readonly IESClientProvider _ESClientProvider;

        public TestController(IOptions<ElasticSetting> elasticSetting,IESClientProvider eSClientProvider)
        {
            _elasticSetting = elasticSetting.Value;
            _ESClientProvider = eSClientProvider;
        }

        [HttpGet]
        public ElasticSetting GetElasticSetting()
        {
            return _elasticSetting;
        }

        [HttpGet]
        public TimeSpan AddBillionData()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //每次存10万条，存1000次
            //for (int i = 0; i < 10000; i++)
            //{
                Task.Factory.StartNew(() =>
                {
                    var lstPerson = new List<Person>();

                    for (int j = 0; j < 10000; j++)
                    {
                        var person = new Person();
                        person.ID = j;
                        person.Age = j;
                        person.Birthday = DateTime.Now.AddSeconds(j);
                        person.Sex = j % 2 == 0;
                        person.Name = $"name_{j}";

                        lstPerson.Add(person);
                    }

                    var response = _ESClientProvider.GetClient("lts").IndexMany(lstPerson);
                }).Wait();
            //}

            sw.Stop();
            return sw.Elapsed;
        }
    }
}
