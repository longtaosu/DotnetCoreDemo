using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SettingApi.Settings;

namespace SettingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        private readonly IOptions<ini_Section0> _ini;
        private readonly IOptions<Xml_Section0> _xml;
        private readonly IOptions<jsonSetting> _json;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IConfiguration config,IOptions<ini_Section0> inisection0,IOptions<Xml_Section0> xmlsection ,IOptions<jsonSetting> jsonSetting)
        {
            _ini = inisection0;
            _xml = xmlsection;
            _json = jsonSetting;

            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            //直接通过 key 读取配置信息
            var sc0 = _config.GetSection("ini_section0:key0");            // ini
            var xsc0 = _config.GetSection("xml_section0:key0");           //xml
            var json0 = _config.GetSection("JsonSet:json1");              //json

            //Bind方法
            ini_Section0 ini = new ini_Section0();
            _config.GetSection("ini_section0").Bind(ini);                 // ini
            Xml_Section0 xml = new Xml_Section0();             
            _config.GetSection("xml_section0").Bind(xml);                 // xml
            jsonSetting json = new jsonSetting();
            _config.GetSection("JsonSet").Bind(json);                     // json

            //Get方法
            ini_Section0 ini1 = _config.GetSection("ini_section0").Get<ini_Section0>();
            Xml_Section0 xml1 = _config.GetSection("xml_section0").Get<Xml_Section0>();
            jsonSetting json1 = _config.GetSection("JsonSet").Get<jsonSetting>();




            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
