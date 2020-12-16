using ElasticModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueryDslPart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryDslPart.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompoundController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;

        public CompoundController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        #region BoolDslComplexQuery

        //[HttpGet]
        //public List<Person> BoolDslComplexQuery()
        //{
        //    var result = _ESClientProvider.GetClient().Search<Person>(s => s.Query(q => q.QueryString(c => c.Name("")))
        //    && s.Query(q => q.SimpleQueryString(c => c.Name("")))
        //    );
        //}
        #endregion

        #region BoolQuery
        /// <summary>
        /// 多条件查询,★★★★★★★★★★
        /// </summary>
        /// <param name="name"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> BoolQuery(string name,int min,int max)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Bool(c => c
                        .Name("BoolQuery")
                        .Should(s=>s.Term(t=>t.Field(f=>f.Name).Value(name)))
                        .Must(m => m.Range(r=>r.Field(f=>f.Age).LessThan(max).GreaterThan(min)))
                        //.MustNot(m => m.MatchAll())
                        //.Filter(f=>f.MatchAll())
                        //.MinimumShouldMatch(1)
                        .Boost(2)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region FluentDSL
        /// <summary>
        /// boosting 查询可以用来有效地降级能匹配给定查询的结果。 与 bool 查询中的“NOT”子句不同，这仍然会选择包含非预期条款的文档，但会降低其总分。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> FluentDSL()
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Boosting(c => c
                        .Name("FluentDSL")
                        .Boost(1.1)
                        .Positive(p=>p.MatchAll(m=>m.Name("filter")))
                        .Negative(p=>p.MatchAll(m=>m.Name("query")))
                        .NegativeBoost(1.12)
                        
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region ConstantScore
        /// <summary>
        /// 此查询可对另一个查询进行包装，并简单地返回一个与该过滤器中每个文档的查询提升相等的常量分数
        /// 搞不懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> ConstantScore()
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .ConstantScore(c => c
                        .Name("ConstantScore")
                        .Boost(1.1)
                        .Filter(f=>f.MatchAll(m=>m.Name("filter")))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region DismaxQuery
        /// <summary>
        /// 此查询生成由其子查询生成的文档的并集，并为每个文档分配由任意子查询生成的该文档的最大分数，以及任何其他匹配子查询的分数增量即 tie breaking 属性。
        /// ★★★★★★★★★★
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> DismaxQuery(string name,int min ,int max)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .DisMax(c => c
                        .Name("DismaxQuery")
                        .Boost(1.1)
                        .TieBreaker(0.11)
                        .Queries(
                            q => q.Term(t => t.Field(f => f.Name).Value(name)),
                            q => q.Range(c => c.Field(f => f.Age).LessThan(max).GreaterThan(min))
                        )
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region FunctionScoreQuery

        #endregion

    }
}
