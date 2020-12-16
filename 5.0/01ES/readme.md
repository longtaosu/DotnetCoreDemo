# 可视化工具

- ElasticHD

```shell
# 下载elasticHD_linux_amd64.zip
# yum install -y unzip zip
unzip elasticHD_linux_amd64.zip
#修改权限：
chmod -R 777 ElasticHD
#运行:
./ElasticHD -p 0.0.0.0:9800
```



# Nuget

- NEST
- Elasticsearch.Net

> 其中 Elasticsearch.Net 更偏底层，无其他依赖。NEST是更高级的封装，较为常用。



# 代码

## 配置

```json
"ES": {
  "uri": "http://172.30.10.191:9200",
  "defaultIndex": "lts"
}
```

配置信息包含 ES 的访问地址、用户名、密码和默认的索引。



## ESProvider

创建ES访问的接口

```c#
public interface IESClientProvider
{
    ElasticClient GetClient();

    ElasticClient GetClient(string index);
}
```

创建接口的实现

```c#
public class ESClientProvider : IESClientProvider
{
    private readonly ElasticSetting _elasticSetting;

    public ESClientProvider(IOptions<ElasticSetting> elasticSetting)
    {
        _elasticSetting = elasticSetting.Value;
    }

    public ElasticClient GetClient()
    {
        var uri = new Uri(_elasticSetting.uri);
        return new ElasticClient(new ConnectionSettings(uri).DefaultIndex(_elasticSetting.defaultIndex));
    }

    public ElasticClient GetClient(string index)
    {
        var uri = new Uri(_elasticSetting.uri);
        return new ElasticClient(new ConnectionSettings(uri).DefaultIndex(index));
    }
}
```

## DI

```c#
public static class ElasticExtension
{
    public static IServiceCollection AddElasticService(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<ElasticSetting>(configuration.GetSection("ES"));
        services.AddScoped<IESClientProvider, ESClientProvider>();

        return services;
    }
}
```



# 基本操作

## 查询

### 查询全部

```c#
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
```

### 精确查询

```c#
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
```

### 模糊查询

```c#
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
```



## 复杂查询

### BoolQuery

```c#
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
```

### SpanMultiTerm

```c#
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
```

### TermsQuery

```c#
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
```

### Regexp

```c#
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
```

## 范围查询

### Range

```c#
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
```

### DateRange

```c#
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
```

## 数据插入

### 单条插入

```c#
[HttpPost]
public bool IndexDocument(Person person)
{
    var indexResponse = _ESClientProvider.GetClient().IndexDocument(person);

    return indexResponse.IsValid;
}
```

### 批量插入

```c#
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
```

## 数据更新

### 全部字段更新

```c#
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
```

### 部分字段更新

> PersonPartial是一个仅包含Person部分字段的类

```c#
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
```





# 参考

http://doc.codingdict.com/elasticsearch/

https://www.knowledgedict.com/tutorial/elasticsearch-intro.html

https://code-maze.com/elasticsearch-aspnet-core/

https://www.cnblogs.com/huhangfei/p/5726650.html