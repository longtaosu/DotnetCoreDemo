using ElasticModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QueryDslPart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// http://doc.codingdict.com/elasticsearch/search.html?q=PrefixQuerypre
/// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/parent-id-query-usage.html
/// </summary>
namespace QueryDslPart.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JoiningController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;

        public JoiningController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        #region HasChild
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> HasChild(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .HasChild<Person>(c => c
                        .Name("HasChild")
                        .Boost(1.1)
                        //.InnerHits(i => i.Explain())
                        //.MaxChildren(5)
                        //.MinChildren(1)
                        .ScoreMode(Nest.ChildScoreMode.Average)
                        .Query(qq => qq.Term(t => t.Field(f => f.Name).Value(name)))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region HasParent
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> HasParent(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .HasParent<Person>(c => c
                        .Name("HasChild")
                        .Boost(1.1)
                        .InnerHits(i => i.Explain())
                        .Score()
                        .Query(qq => qq.Term(t => t.Field(f => f.Name).Value(name)))
                        .IgnoreUnmapped()
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region NestedQuery
        /// <summary>
        /// 一种对象类型的特殊版本，它允许索引对象数组，独立地索引每个对象
        /// 运行失败
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Person> NestedQuery(List<string> names)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Nested(c => c
                        .Name("NestedQuery")
                        .Boost(1.1)
                        .InnerHits(i => i.Explain())
                        .Path(p => p.Name)
                        .Query(nq => nq.Terms(t => t.Field(f => f.Name).Terms(names)))
                        //.IgnoreUnmapped()
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region ParentIdQuery
        [HttpGet]
        public List<Person> ParentIdQuery(int id)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .ParentId(c => c
                        .Name("ParentId")
                        .Type<Person>()
                        .Id(id)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

    }
}
