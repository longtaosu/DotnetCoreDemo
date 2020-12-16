using ElasticModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using QueryDslPart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryDslPart.Controllers
{
    /// <summary>
    /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/term-level-queries.html
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TermLevelController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;
        public TermLevelController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        #region ExistQuery
        [HttpGet]
        public List<Person> ExistQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SimpleQueryString(c => c
                        .Name("ExistQuery")
                        .Boost(1.1)
                        .Fields(f=>f.Field(p=>p.Name).Field("myOtherField"))
                        .Query(name)
                        .Analyzer("standard")
                        .DefaultOperator(Operator.Or)
                        .Flags(SimpleQueryStringFlags.And | SimpleQueryStringFlags.Near)
                        .Lenient()
                        .AnalyzeWildcard()
                        .MinimumShouldMatch("30%")
                        .FuzzyPrefixLength(0)
                        .FuzzyMaxExpansions(50)
                        .FuzzyTranspositions()
                        .AutoGenerateSynonymsPhraseQuery(false)
                        )
                    )                
                );

            return result.Documents.ToList();
        }
        #endregion

        #region FuzzyDateQuery
        /// <summary>
        /// 没搞明白
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> FuzzyDateQuery()
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .FuzzyDate(c => c
                        .Name("FuzzyDate")
                        .Boost(1.1)
                        .Field(p=>p.Birthday)
                        .Fuzziness(TimeSpan.FromDays(2))
                        .Value(DateTime.Now.AddDays(-6.5))
                        .MaxExpansions(100)
                        .PrefixLength(3)
                        .Rewrite(MultiTermQueryRewrite.ConstantScore)
                        .Transpositions()
                        )            
                    )
                );
            return result.Documents.ToList();
        }
        #endregion

        #region FuzzyNumericQuery
        /// <summary>
        /// 没搞明白
        /// </summary>
        /// <param name="age"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> FuzzyNumericQuery(int age)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .FuzzyNumeric(c => c
                        .Name("FuzzyNumeric")
                        .Boost(1.1)
                        .Field(p => p.Age)
                        .Fuzziness(200)
                        .Value(age)
                        .MaxExpansions(100)
                        .PrefixLength(3)
                        .Rewrite(MultiTermQueryRewrite.ConstantScore)
                        .Transpositions()
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region FuzzyQuery
        [HttpGet]
        public List<Person> FuzzyQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Fuzzy(c=>c
                        .Name("FuzzyQuery")
                        .Boost(1.1)
                        .Field(p=>p.Name)
                        .Fuzziness(Fuzziness.Auto)
                        .Value(name)
                        .MaxExpansions(100)
                        .PrefixLength(3)
                        .Rewrite(MultiTermQueryRewrite.ConstantScore)
                        .Transpositions()
                    )                    
                )            
            );

            return result.Documents.ToList();
        }
        #endregion

        #region IdsQuery
        /// <summary>
        /// 仅仅过滤与所提供的ID相匹配的文档，查询使用 _uid 字段
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public List<Person> IdsQuery(List<int> ids)
        {
            List<string> lstIds = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                lstIds.Add(ids[i].ToString());
            }

            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Ids(c => c
                        .Name("Ids")
                        .Boost(1.1)
                        .Values(lstIds)
                    )
                )
            );

            return result.Documents.ToList();
        }
        #endregion

        #region PrefixQuery
        /// <summary>
        /// 前缀查询，匹配包含具有指定前缀的项 ★★★★★
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> PrefixQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Prefix(c => c
                        .Name("")
                        .Boost(1.1)
                        .Field(p=>p.Name)
                        .Value(name)
                        .Rewrite(MultiTermQueryRewrite.TopTerms(10))
                    )
                )            
            );

            return result.Documents.ToList();
        }
        #endregion

        #region DateRangeQuery
        /// <summary>
        /// 根据限定时间段筛选数据 ★★★
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> DateRangeQuery(DateTime start,DateTime end)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .DateRange(c=>c
                        .Name("DateRangeQuery")
                        .Boost(1.1)
                        .Field(f=>f.Birthday)
                        .GreaterThan(start)
                        //.GreaterThanOrEquals()
                        .LessThan(end)                        
                    )            
                )            
            );

            return result.Documents.ToList();
        }
        #endregion

        #region LongRangeQuery
        /// <summary>
        /// 数值范围内数据筛选，范围值类型 long ★★★
        /// </summary>
        /// <param name="lowAge"></param>
        /// <param name="highAge"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> LongRangeQuery(int lowAge,int highAge)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .LongRange(c=>c
                        .Name("LongRangeQuery")
                        .Boost(1.1)
                        .Field(f=>f.Age)
                        .GreaterThan(lowAge)
                        .LessThan(highAge)
                        .Relation(RangeRelation.Within)
                    )                    
                )            
            );

            return result.Documents.ToList();
        }
        #endregion

        #region NumericRangeQuery
        /// <summary>
        /// 数值范围内数据筛选，范围值类型 double ★★★★
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> NumericRangeQuery(int min, int max)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Size(20)
                .Query(q => q
                    .Range(c=>c
                        .Name("NumericRange")
                        .Boost(1.1)
                        .Field(f=>f.Age)
                        .GreaterThan(min)
                        .LessThan(max)
                        .Relation(RangeRelation.Within)
                    )
                )            
            );

            return result.Documents.ToList();
        }
        #endregion

        #region TermRangeQuery
        /// <summary>
        /// 字符串范围内数据筛选，范围值类型 string ★★★★
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> TermRangeQuery(string min ,string max)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Size(50)
                .Query(q => q
                    .TermRange(c=>c
                        .Name("TermRangeQuery")
                        .Boost(1.1)
                        .Field(f=>f.Name)
                        .GreaterThan(min)
                        .LessThan(max)
                    )
                )
            );

            return result.Documents.ToList();
        }
        #endregion

        #region RegexpQuery
        /// <summary>
        /// 正则表达式查询：name_*23 ，★★★★★
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> RegexpQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Regexp(c=>c
                        .Name("RegexpQuery")
                        .Boost(1.1)
                        .Field(f=>f.Name)
                        .Value(name)
                        .Flags("INTERSECTION|COMPLEMENT|EMPTY")
                        .MaximumDeterminizedStates(20000)
                        .Rewrite(MultiTermQueryRewrite.TopTerms(10))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region TermQuery
        /// <summary>
        /// 字段的准确匹配，相等筛选，★★★★
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> TermQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Term(c => c
                        .Name("TermQuery")
                        .Boost(1.1)
                        .Field(f=>f.Name)
                        .Value(name)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region TermsSetQuery
        [HttpPost]
        public List<Person> TermsSetQuery(string[] name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .TermsSet(c => c
                        .Name("TermsSet")
                        .Boost(1.1)
                        .Field(p=>p.Name)
                        .Terms(name)
                        //.MinimumShouldMatchField(p=>p.Name)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region TermsListQuery
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Person> TermsListQuery(List<string> names)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Terms(c => c
                        .Name("TermsListQuery")
                        .Boost(1.1)
                        .Field(p=>p.Name)
                        .Terms(names)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region TermsLookup
        /// <summary>
        /// 没搞懂
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> TermsLookup(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Terms(c => c
                        .Name("TermsLookup")
                        .Boost(1.1)
                        .Field(p=>p.Name)
                        .TermsLookup<Person>(e=>e
                            .Path(p=>p.Name)
                            .Id(name)
                            //.Routing("myRoutingValue")
                            )
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region TermsQuery
        /// <summary>
        /// 筛选所拥有的字段能够匹配提供的多个项（not analyzed）中任意一个的文档。★★★★
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        [HttpPost]
        public List<Person> TermsQuery(List<string> names)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Terms(c => c
                        .Name("TermsQuery")
                        .Boost(1.1)
                        .Field(p=>p.Name)
                        .Terms(names)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region WildcardQuery
        /// <summary>
        /// 通配符查询，通配符包括： *,? 
        /// 需要注意，通配符项不应以 *,? 开头。★★★★★
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> WildcardQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Wildcard(c => c
                        .Name("Wildcard")
                        .Boost(1.1)
                        .Field(p=>p.Name)
                        .Value(name)
                        .Rewrite(MultiTermQueryRewrite.TopTermsBoost(10))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

    }
}
