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
    public class SummaryController : ControllerBase
    {
        #region 初始化
        private readonly IESClientProvider _ESClientProvider;
        public SummaryController(IESClientProvider eSClientProvider)
        {
            _ESClientProvider = eSClientProvider;
        }
        #endregion

        #region 简单查询
        [HttpGet]
        public List<Person> GetAllPersons()
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Size(100)
                .Query(q => q
                    .MatchAll()
                    )
                );

            return result.Documents.ToList();
        }

        /// <summary>
        /// 精确查询，使用Term Query实现精确查询
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


        /// <summary>
        /// 用法类似 Term
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> Match(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(name)
                        )
                    )
                );

            return result.Documents.ToList();
        }


        /// <summary>
        /// 模糊查询，使用 Wildcard 实现模糊查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> Wildcard(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Wildcard(w => w
                        .Name("Wildcard")
                        .Field(f => f.Name)
                        .Value($"*{name}*")
                        //.Rewrite(MultiTermQueryRewrite.TopTermsBoost(10))
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region 复杂查询
        #region BoolQuery
        /// <summary>
        /// 多条件查询
        /// </summary>
        /// <param name="name"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> BoolQuery(string name, int min, int max)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Bool(c => c
                        .Name("BoolQuery")
                        .Should(s => s.Term(t => t.Field(f => f.Name).Value(name)))
                        .Must(m => m.Range(r => r.Field(f => f.Age).LessThan(max).GreaterThan(min)))
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
                        .Field(p => p.Name)
                        .Terms(names)
                        )
                    )
                );

            return result.Documents.ToList();
        }
        #endregion

        #region RegexpQuery
        /// <summary>
        /// 正则表达式查询：name_*23* ，★★★★★
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<Person> RegexpQuery(string name)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .Regexp(c => c
                        .Name("RegexpQuery")
                        .Boost(1.1)
                        .Field(f => f.Name)
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
                    .Range(c => c
                        .Name("NumericRange")
                        .Boost(1.1)
                        .Field(f => f.Age)
                        .GreaterThan(min)
                        .LessThan(max)
                        .Relation(RangeRelation.Within)
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
        public List<Person> DateRangeQuery(DateTime start, DateTime end)
        {
            var result = _ESClientProvider.GetClient().Search<Person>(s => s
                .Query(q => q
                    .DateRange(c => c
                        .Name("DateRangeQuery")
                        .Boost(1.1)
                        .Field(f => f.Birthday)
                        .GreaterThan(start)
                        //.GreaterThanOrEquals()
                        .LessThan(end)
                    )
                )
            );

            return result.Documents.ToList();
        }
        #endregion

        #endregion

        #region 单条插入
        [HttpPost]
        public bool IndexDocument(Person person)
        {
            var indexResponse = _ESClientProvider.GetClient().IndexDocument(person);

            return indexResponse.IsValid;
        }
        #endregion

        #region 批量插入
        [HttpPost]
        public bool IndexMany()
        {
            //每次存10万条，存1000次
            for (int i = 0; i < 10000; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var lstPerson = new List<Person>();

                    for (int j = 0; j < 10000; j++)
                    {
                        var person = new Person();
                        person.ID = i * 10000 + j;
                        person.Age = i * 10000 + j;
                        person.Birthday = DateTime.Now.AddSeconds(j);
                        person.Sex = j % 2 == 0;
                        person.Name = $"name_{ i * 10000 + j}";

                        lstPerson.Add(person);
                    }

                    var response = _ESClientProvider.GetClient().IndexMany(lstPerson);
                }).Wait();
            }

            return true;
        }
        #endregion

        #region 更新数据
        /// <summary>
        /// 全部字段更新，未指明的字段会变为默认值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public bool Update(int id)
        {
            var result = _ESClientProvider.GetClient().Update<Person, object>(1, p => p
                .Doc(new Person { Name = "name_1" })
                );
            return result.IsValid;
        }

        /// <summary>
        /// 全部字段更新，未指明的字段会变为默认值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public bool DocAsUpsert(int id)
        {
            var result = _ESClientProvider.GetClient().Update<Person, object>(DocumentPath<Person>.Id(id), p => p
                    .Doc(new Person { Name = $"name_{id}_update", Birthday = DateTime.Now })
                    .DocAsUpsert()
                    .Refresh(Elasticsearch.Net.Refresh.True)
                );

            return result.IsValid;
        }
        /// <summary>
        /// 使用匿名类，该方法将包含部分字段的类使用object匿名类型代替
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdatePartial(int id)
        {
            var result = _ESClientProvider.GetClient().Update<Person, object>(id, p => p
                    .Doc(new { Name = $"name_{id}", Birthday = DateTime.Now })
                );
            return result.IsValid;
        }

        /// <summary>
        /// 部分字段更新，该方法需要创建一个只包含部分字段的类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdatePartial2(int id)
        {
            var request = new UpdateRequest<Person, PersonPartial>(id)
            {
                Doc = new PersonPartial()
                {
                    Birthday = DateTime.Now
                }
            };
            var result = _ESClientProvider.GetClient().Update(request);

            return result.IsValid;
        }
        #endregion

        #region 删除
        //[HttpGet]
        //public bool Delete(int id)
        //{

        //    //DeleteRequest request = new DeleteRequest(new Id(id));

        //    var request = new UpdateByQueryRequest()
        //    {
                
        //    };
        //    var result = _ESClientProvider.GetClient().Delete<Person>()
        //}
        #endregion
    }

    public class PersonPartial
    {
        public DateTime Birthday { get; set; }
    }
}
