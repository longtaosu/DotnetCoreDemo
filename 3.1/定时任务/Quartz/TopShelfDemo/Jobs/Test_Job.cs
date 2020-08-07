using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TopShelfDemo.Base;
using TopShelfDemo.Services;

namespace TopShelfDemo.Jobs
{
    public class Test_Job : IJob
    {
        private ILogger _logger;
        private AppSettings _appSettings;
        private ITestService _testService;
        public Test_Job(IOptions<AppSettings> appSettings,ILogger<Test_Job> logger,ITestService testService)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _testService = testService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"执行时间：{DateTime.Now.ToString()}");

            _logger.LogInformation($"执行时间：{DateTime.Now.ToString()}");

            var data = _testService.DataAccess();

            return Task.CompletedTask;
        }

        private void GetData()
        {
            var client = new RestClient(_appSettings.Api.ServiceUrl);
            var request = new RestRequest(_appSettings.Api.api_url, Method.POST);
            request.AddHeader("Authorization", "Bearer token");
            var data = new
            {
                userIDs = new int[] { 0 },
                orgCode = "string",
                beginTime = "2020-08-07T01:33:35.157Z",
                endTime = "2020-08-07T01:33:35.157Z",
                isEnable = 0,
                page = 0,
                pageSize = 0
            };
            request.AddJsonBody(data);
            var result = client.Post<CSDataResult>(request);
            var vv = result.Data.Data;
        }
    }
}
