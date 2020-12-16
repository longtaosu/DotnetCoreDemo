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
    public class SpecializedController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;

        public SpecializedController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        #region DistanceFeatureQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> DistanceFeatureQuery()
        {
            return new List<Person>();
        }
        #endregion

        #region MoreLikeThisFullDocument
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Person> MoreLikeThisFullDocument(Person person)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .MoreLikeThis(c => c
                        .Name("MoreLikeThisFullDocument")
                        .Fields(fs => fs
                            .Field(f => f.Name)
                            .Field(f => f.Sex)
                            )
                        .Like(l => l.Document(d => d
                              .Document(person)
                              .Routing(person.Name)
                            )
                        .Text(person.Name)
                        )

                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region MoreLikeThisQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <param name="person"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Person> MoreLikeThisQuery(Person person,string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .MoreLikeThis(c => c
                        .Name("MoreLikeThis")
                        .Boost(1.1)
                        .Like(l => l.Document(d => d.Id(person.Name).Routing(person.Name)).Text(name))
                        .Analyzer("some_analyzer")
                        .BoostTerms(1.1)
                        .Include()
                        .MaxDocumentFrequency(12)
                        .MaxQueryTerms(12)
                        .MaxWordLength(300)
                        .MinDocumentFrequency(1)
                        .MinTermFrequency(1)
                        .MinWordLength(10)
                        .StopWords("and", "the")
                        .MinimumShouldMatch(1)
                        .Fields(f => f.Field(p => p.Name))
                        .Unlike(l => l.Text("not like this text"))
                        )
                    )
                ); 

            return result.Documents.ToList();
        }
        #endregion

        #region PercolateQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Person> PercolateQuery(Person person)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Percolate(c => c
                        .Name("PercolateQuery")
                        .Document(person)
                        .Field(f=>f.Name)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region PinnedQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> PinnedQuery()
        {
            return new List<Person>();
        }
        #endregion

        #region RankFeatureQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> RankFeatureQuery()
        {
            return new List<Person>();
        }
        #endregion

        #region ScriptScoreQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> ScriptScoreQuery()
        {
            return new List<Person>();
        }
        #endregion

        #region ScriptQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> ScriptQuery()
        {
            return new List<Person>();
        }
        #endregion

        #region ShapeQuery
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> ShapeQuery()
        {
            return new List<Person>();
        }
        #endregion



    }
}
