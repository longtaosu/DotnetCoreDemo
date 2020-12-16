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
    /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/full-text-queries.html
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FullTextController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;

        public FullTextController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        /// <summary>
        /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/common-terms-usage.html
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        #region CommonTerms 在不牺牲性能的情况下替代停用词提高搜索准确率和召回率的方案
        [HttpGet]
        public List<Person> CommonTerms1(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Size(10)
                .Query(q => q.CommonTerms(c => c
                      .Field(f => f.Name)
                      .Analyzer("standard")
                      .Boost(1.1)
                      .CutoffFrequency(0.001)
                      .HighFrequencyOperator(Nest.Operator.And)
                      .LowFrequencyOperator(Nest.Operator.Or)
                      .MinimumShouldMatch(1)
                      .Query(name)
                    )
                )
            );

            return result.Documents.ToList();
        }


        /// <summary>
        /// 该方案已过时
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //[HttpGet]
        //public List<Person> CommonTerms2(string name)
        //{
        //    var query = new CommonTermsQuery()
        //    {
        //        Field = Infer.Field<Person>(p => p.Name),
        //        Analyzer = "standard",
        //        Boost = 1.1,
        //        CutoffFrequency = 0.001,
        //        HighFrequencyOperator = Operator.And,
        //        LowFrequencyOperator = Operator.Or,
        //        MinimumShouldMatch = 1,
        //        Query = name
        //    };

        //    var result = _ESClientProvider.GetClient().Search<Person>(query);

        //    return result.Documents.ToList();
        //}
        #endregion


        #region Intervals Usage
        [HttpGet]
        public List<Person> Intervals1(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Size(10)
                .Query(q => q.Intervals(c => c
                      .Field(p => p.Name)
                      .Name("intervals")
                      .Boost(1.1)
                      .AnyOf(any => any.Intervals(i => i
                            .Match(m => m
                                .Query(name)
                                .MaxGaps(5)
                                .Ordered()
                                .Filter(f => f
                                    .Containing(co => co
                                        .Match(mm => mm
                                            .Query(name)
                                    )
                                  )
                                )
                              )
                            .AllOf(all => all
                                .Intervals(ii => ii
                                    .Match(m => m.Query(name))
                                    .Match(m => m.Query(name))
                                )
                                .Filter(f => f
                                    .Script(s => s
                                        .Source("interval.start > 0 && interval.end < 200")))
                             )
                          )
                        )
                     )
                   )
                );

            return result.Documents.ToList();
        }

        //[HttpGet]
        //public List<Person> Intervals2(string name)
        //{
        //    var query = new IntervalsQuery()
        //    {
        //        Field = Infer.Field<Person>(p=>p.Name),
        //        Name = "intervals",
        //        Boost = 1.1,
        //        AnyOf = new IntervalsAnyOf
        //        {
        //            Intervals = new IntervalsContainer[]
        //            {
        //                new IntervalsMatch
        //                {
        //                    Query = name,
        //                    MaxGaps = 5,
        //                    Ordered = true,
        //                    Filter = new IntervalsFilter
        //                    {
        //                        Containing = new IntervalsMatch
        //                        {
        //                            Query = name
        //                        }
        //                    }
        //                },
        //                new IntervalsAllOf
        //                {
        //                    Intervals = new IntervalsContainer[]
        //                    {
        //                        new IntervalsMatch
        //                        {
        //                            Query = name,
        //                        },
        //                        new IntervalsMatch
        //                        {
        //                            Query = name,
        //                        }
        //                    },
        //                    Filter = new IntervalsFilter
        //                    {
        //                        Script = new InlineScript("interval.start > 0 && interval.end < 200")
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    var result = _ESClientProvider.GetClient().Search<Person>(s => s.Query(q => q.Intervals(query)));//(query as ISearchRequest);

        //    return result.Documents.ToList();
        //}
        #endregion

        #region MatchBoolPrefix
        /// <summary>
        /// 前缀匹配
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> MatchBoolPrefix1(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Size(10)
                .Query(q=>q
                    .MatchBoolPrefix(c=>c
                        .Field(p=>p.Name)
                        .Analyzer("standard")
                        .Boost(1.1)
                        .Query(name)
                        .Fuzziness(Fuzziness.AutoLength(3,6))
                        .FuzzyTranspositions()
                        .FuzzyRewrite(MultiTermQueryRewrite.TopTermsBlendedFreqs(10))
                        .Name("MatchBoolPrefix")
                    )
                )
            );

            return result.Documents.ToList();
        }

        //[HttpGet]
        //public List<Person> MatchBoolPrefix2(string name)
        //{
        //    var query = new MatchBoolPrefixQuery()
        //    {
        //        Field = Infer.Field<Person>(p => p.Name),
        //        Analyzer = "standard",
        //        Boost = 1.1,
        //        Name = "MatchBoolPrefix",
        //        Query = name,
        //        Fuzziness = Fuzziness.AutoLength(3, 6),
        //        FuzzyTranspositions = true,
        //        FuzzyRewrite = MultiTermQueryRewrite.TopTermsBlendedFreqs(10)
        //    };

        //}
        #endregion

        #region MatchPhrasePrefix
        [HttpGet]
        public List<Person> MatchPhrasePrefix1(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .MatchPhrasePrefix(m=>m
                        .Field(f=>f.Name)
                        .Analyzer("standard")
                        .Boost(1.1)
                        .Query(name)
                        .MaxExpansions(2)
                        .Slop(2)
                        .Name("MatchPhrasePrefix")
                        )                        
                    )
                );

            return result.Documents.ToList();
        }

        //public List<Person> MatchPhrasePrefix2(string name)
        //{
        //    var query = new MatchPhrasePrefixQuery()
        //    {
        //        Field = Infer.Field<Person>(p=>p.Name),
        //        Analyzer = "standard",
        //        Boost = 1.1,
        //        Name = "MatchPhrasePrefix",
        //        Query = name,
        //        MaxExpansions = 2,
        //        Slop = 2
        //    };
        //}
        #endregion

        #region MatchPhrase
        [HttpGet]
        public List<Person> MatchPhrase1(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q=>q
                    .MatchPhrase(m=>m
                        .Field(f=>f.Name)
                        .Analyzer("standard")
                        .Boost(1.1)
                        .Query(name)
                        .Slop(2)
                        .Name("MatchPhrase")
                    )                        
                )
            );

            return result.Documents.ToList();
        }

        //public List<Person> MatchPhrase2(string name)
        //{
        //    var query = new MatchPhraseQuery()
        //    {
        //        Field = Infer.Field<Person>(f => f.Name),
        //        Analyzer = "standard",
        //        Boost = 1.1,
        //        Name = "MatchPhrase",
        //        Query = name,
        //        Slop = 2
        //    };
        //}
        #endregion

        #region Match
        [HttpGet]
        public List<Person> Match1(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Analyzer("standard")
                        .Boost(1.1)
                        .Query(name)
                        .Fuzziness(Fuzziness.AutoLength(3,6))
                        .Lenient()
                        .FuzzyTranspositions()
                        .MinimumShouldMatch(2)
                        .Operator(Operator.Or)
                        .FuzzyRewrite(MultiTermQueryRewrite.TopTermsBlendedFreqs(10))
                        .Name("Match")
                        .AutoGenerateSynonymsPhraseQuery(false)
                        )
                    )
                );

            return result.Documents.ToList();
        }

        //[HttpGet]
        //public List<Person> Match2(string name)
        //{
        //    var query = new MatchQuery()
        //    {
        //        Field = Infer.Field<Person>(p=>p.Name),
        //        Analyzer = "standard",
        //        Boost = 1.1,
        //        Name = "Match",
        //        Query = name,
        //        Fuzziness = Fuzziness.AutoLength(3,6),
        //        FuzzyTranspositions = true,
        //        MinimumShouldMatch = 2,
        //        FuzzyRewrite = MultiTermQueryRewrite.TopTermsBlendedFreqs(10),
        //        Lenient = true,
        //        Operator = Operator.Or,
        //        AutoGenerateSynonymsPhraseQuery = false
        //    };
        //}
        #endregion

        #region MultiMatch
        [HttpGet]
        public List<Person> MultiMatch1(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .MultiMatch(m=>m
                        .Fields(f=>f.Field(p=>p.Name).Field(p=>p.Name))
                        .Query(name)
                        .Analyzer("standard")
                        .Boost(1.1)
                        .Slop(2)
                        .Fuzziness(Fuzziness.Auto)
                        .PrefixLength(2)
                        .MaxExpansions(2)
                        .Operator(Operator.Or)
                        .MinimumShouldMatch(2)
                        .FuzzyRewrite(MultiTermQueryRewrite.ConstantScoreBoolean)
                        .TieBreaker(1.1)
                        .CutoffFrequency(0.001)
                        .Lenient()
                        .ZeroTermsQuery(ZeroTermsQuery.All)
                        .Name("MultiMatch")
                        .AutoGenerateSynonymsPhraseQuery(false)                    
                        )                    
                    )                
                );

            return result.Documents.ToList();
        }

        //public List<Person> MultiMatch2(string name)
        //{

        //    var query = new MultiMatchQuery()
        //    {
        //        Fields = Infer.Fields<Person>(p => p.Name).And("myOtherField"),
        //        Query = name,
        //        Analyzer = "standard",
        //        Boost = 1.1,
        //        Slop = 2,
        //        Fuzziness = Fuzziness.Auto,
        //        PrefixLength = 2,
        //        MaxExpansions = 2,
        //        Operator = Operator.Or,
        //        MinimumShouldMatch = 2,
        //        FuzzyRewrite = MultiTermQueryRewrite.ConstantScoreBoolean,
        //        TieBreaker = 1.1,
        //        CutoffFrequency = 0.001,
        //        Lenient = true,
        //        ZeroTermsQuery = ZeroTermsQuery.All,
        //        Name = "MultiMatch",
        //        AutoGenerateSynonymsPhraseQuery = false
        //    };
        //}
        #endregion

        #region QueryString
        [HttpGet]
        public List<Person> QueryString(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s.Query(q => q
                .QueryString(c => c
                    .Name("QueryString")
                    .Boost(1.1)
                    .Fields(f => f.Field(f => f.Name).Field("myOtherField"))
                    .Query(name)
                    .DefaultOperator(Operator.Or)
                    .Analyzer("standard")
                    .QuoteAnalyzer(name)
                    .AllowLeadingWildcard()
                    .MaximumDeterminizedStates(2)
                    .Escape()
                    .FuzzyPrefixLength(2)
                    .FuzzyMaxExpansions(3)
                    .FuzzyRewrite(MultiTermQueryRewrite.ConstantScore)
                    .Rewrite(MultiTermQueryRewrite.ConstantScore)
                    .Fuzziness(Fuzziness.Auto)
                    .TieBreaker(1.2)
                    .AnalyzeWildcard()
                    .MinimumShouldMatch(2)
                    .QuoteFieldSuffix("'")
                    .Lenient()
                    .AutoGenerateSynonymsPhraseQuery(false)
                    )
                )
            );

            return result.Documents.ToList();
        }
        #endregion

        #region SimpleQueryString
        [HttpGet]
        public List<Person> SimpleQueryString(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .SimpleQueryString(c=>c
                        .Name("SimpleQueryString")
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

    }
}
