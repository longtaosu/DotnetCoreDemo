using ElasticModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using SearchPart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchPart.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ExplainController : ControllerBase
    {
        private readonly IESClientProvider _ESClientProvider;
        public ExplainController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }

        [HttpGet]
        public object TestExplain()
        {
            var searchRequest = new SearchRequest<Person>()
            {
                Explain = true
            };

            var result = _ESClientProvider.GetClient().Search<Person>(searchRequest);
            return result.Documents.ToList();//..Documents.ToList();
        }
    }
}
