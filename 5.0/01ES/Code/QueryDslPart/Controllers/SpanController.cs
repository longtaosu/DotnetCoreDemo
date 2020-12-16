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
    public class SpanController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;

        public SpanController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        #region SpanContainingQuery
        /// <summary>
        /// 返回包含另一个span查询的匹配项。
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> SpanContainingQuery(string name,int age)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanContaining(c => c
                        .Name("SpanContainingQuery")
                        .Boost(1.1)
                        .Little(i => i.SpanTerm(st => st.Field(f => f.Name).Value(name)))
                        .Big(b => b.SpanTerm(st => st.Field(f => f.Age).Value(age)))
                        
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanFieldMaskingQuery
        /// <summary>
        /// 包装器允许span查询通过欺骗他们的搜索字段来参与复合单字段跨度查询。跨域字段屏蔽查询映射到Lucene的SpanFieldMaskingQuery
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> SpanFieldMaskingQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanFieldMasking(c => c
                        .Name("SpanContainingQuery")
                        .Boost(1.1)
                        .Query(sq => sq.SpanTerm(st => st.Field(p => p.Name).Value(name)))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanFirstQuery
        /// <summary>
        /// 匹配跨越一个字段的开头。
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> SpanFirstQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanFirst(c => c
                        .Name("SpanFirstQuery")
                        .Boost(1.1)
                        .Match(m=>m.SpanTerm(st=>st.Field(f=>f.Name).Value(name)))
                        .End(3)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanMultiTermQuery
        /// <summary>
        /// span_multi查询允许您将多项查询（通配符，模糊，前缀，范围或正则表达式查询之一）包装为Span查询，因此可以嵌套。
        /// name_?123，可以使用多种模糊查询的组合进行查询。★★★★★
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> SpanMultiTermQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanMultiTerm(c => c
                        .Name("SpanMultiTermQuery")
                        .Boost(1.1)
                        //.Match(m => m.Prefix(p => p.Field(f => f.Name).Value(name)))
                        .Match(m => m.Wildcard(w => w.Field(f => f.Name).Value(name)))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanNearQuery
        /// <summary>
        /// 彼此接近的匹配跨距。可以指定坡度，中间不匹配位置的最大数量，以及匹配是否需要排序。
        /// 没搞懂
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> SpanNearQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanNear(c => c
                        .Name("SpanNearQuery")
                        .Boost(1.1)
                        .Clauses(
                            //c=>c.SpanTerm(st=>st.Field(f=>f.Name).Value(name)),
                            c=>c.SpanTerm(st=>st.Field(f=>f.Name).Value(name))                     
                        )
                        .Slop(12)
                        .InOrder()
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanNotQuery
        /// <summary>
        /// 删除与另一个span查询重叠的匹配项。
        /// 没搞懂
        /// </summary>
        /// <param name="name"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> SpanNotQuery(string name,int age)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanNot(c => c
                        .Name("SpanNotQuery")
                        .Boost(1.1)
                        .Post(13)
                        .Pre(14)
                        .Include(i => i.SpanTerm(st => st.Field(f => f.Name).Value(name)))
                        //.Exclude(e => e.SpanTerm(st => st.Field(f => f.Age).Value(age)))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanOrQuery
        [HttpGet]
        public List<Person> SpanOrQuery(string name1,string name2)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanOr(c => c
                        .Name("SpanOr")
                        .Boost(1.1)
                        .Clauses(
                            c => c.SpanTerm(st => st.Field(f => f.Name).Value(name1)),
                            c => c.SpanTerm(st => st.Field(f => f.Name).Value(name2))
                        )
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanTermQuery
        [HttpGet]
        public List<Person> SpanTermQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanTerm(c => c
                        .Name("SpanTermQuery")
                        .Boost(1.1)
                        .Field(f=>f.Name)
                        .Value(name)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region SpanWhthinQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> SpanWithinQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SpanWithin(c => c
                        .Name("SpanWithinQuery")
                        .Boost(1.1)
                        .Little(i=>i.SpanTerm(st=>st.Field(f=>f.Name).Value(name)))
                        .Big(b=>b.SpanTerm(st=>st.Field(f=>f.Name).Value(name)))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

    }
}
