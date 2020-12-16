using ElasticModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchPart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchPart.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;
        public SearchController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        #region Query
        /// <summary>
        /// 精确查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> Query(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(c => c
                    .Term(t => t.Name, name)
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region 

        #endregion


    }
}
