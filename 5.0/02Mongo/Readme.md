# Nuget

- MongoDB.Bson
- MongoDB.Driver



# 配置

```json
"MongoDb": {
  "ConnectionString": "mongodb://172.30.10.90:27017",
  "DbName": "Test",
}
```



# 配置服务

```c#
public static IServiceCollection AddMongoService(this IServiceCollection services,IConfiguration configuration)
{
    var mongoCoon = configuration.GetValue<string>("MongoDb:ConnectionString");
    var mongoDatabase = configuration.GetValue<string>("MongoDb:DbName");
    services.AddSingleton<IMongoDatabase>(m => new MongoClient(mongoCoon).GetDatabase(mongoDatabase));

    return services;
}

//Startup中进行注入
services.AddMongoService(Configuration);
```



# 基本操作

## 构造注入

```c#
private IMongoDatabase _database;
public SummaryController(IMongoDatabase database)
{
    _database = database;
}
```



## Collection

获取指定DB下的Collection

```c#
[HttpGet]
public List<string> GetDbCollections()
{
    var result = _database.ListCollectionNames().ToList();
    return result;
}
```



## 新增

单条插入

```c#
[HttpPost]
public List<Person> InsertOne(Person person)
{
    try
    {
        _database.GetCollection<Person>(_collectionName).InsertOne(person);
        return _database.GetCollection<Person>(_collectionName).Find(t => true).ToList();
    }
    catch (Exception ex)
    {
        return null;
    }
}
```

批量插入

```c#
//循环100次，每次插入10000条数据
[HttpGet]
public void InsertMany()
{
    List<Person> lstData = new List<Person>();
    for (int i = 0; i < 100; i++)
    {
        lstData.Clear();
        for (int j = i * 10000; j < (i + 1) * 10000; j++)
        {
            var person = new Person
            {
                ID = i,
                Age = i,
                Name = $"name_{j}",
                Birthday = DateTime.Now.AddSeconds(i),
                Sex = i % 2 == 0
            };
            lstData.Add(person);
        }
        _database.GetCollection<Person>(_collectionName).InsertMany(lstData);
    }
}
```



## 删除

单条删除

```c#
[HttpGet]
public bool DeletePerson(int id)
{
    var result = _database.GetCollection<Person>(_collectionName).DeleteOne(x => x.ID == id);
    return result.DeletedCount > 0;
}
```

批量删除

```c#
[HttpGet]
public bool DeleteMany(bool sex)
{
    var result = _database.GetCollection<Person>(_collectionName).DeleteMany(x => x.Sex == sex);
    return result.DeletedCount > 0;
}
```



## 更新

```c#
[HttpGet]
public bool Update(int id)
{
    var filter = Builders<Person>.Filter.Where(t => t.ID == id);
    var update = Builders<Person>.Update.Set(x => x.Sex, true);

    return _database.GetCollection<Person>(_collectionName).UpdateMany(filter, update).ModifiedCount > 0;
}
```



## 查询

模糊查询，普通查询可在该基础上简化

```c#
//使用 name 进行模糊查询
//方法1：
[HttpGet]
public List<Person> Complex(string name)
{
    var result = _database.GetCollection<Person>("Person").AsQueryable().Where(t => t.Name.Contains(name)).ToList();
    return result;
}

//方法2：
[HttpGet]
public List<Person> Complex1(string name)
{
    var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
    var result = _database.GetCollection<Person>("Person").Find(filter).ToList();
    return result;
}

//方法3：
[HttpGet]
public List<Person> Complex2(string name)
{
    //var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
    var result = _database.GetCollection<Person>("Person").Find(t => t.Name.Contains(name)).ToList();
    return result;
}
```



## 查询数量

```c#
//获取指定条件下的数据总量
[HttpGet]
public long CountDocuments(string name)
{
    var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
    var result = _database.GetCollection<Person>(_collectionName).Find(filter).CountDocuments();

    return result;
}

//获取Collection中数据的总量
[HttpGet]
public long EstimatedDocumentCount()
{
    var response = _database.GetCollection<Person>(_collectionName).EstimatedDocumentCount();
    return response;
}
```



## 分页、排序

```c#
[HttpGet]
public List<Person> QueryByOrder(bool sex)
{
    var result = _database.GetCollection<Person>("LargePerson").Find(t => t.Sex == sex).SortBy(t => t.Name).Limit(10).ToList();
    return result;
}
```

关联查询

```c#
[HttpGet]
public List<Person> RelateQuery()
{
    var person = _database.GetCollection<Person>(_collectionName);
    var sex = _database.GetCollection<Sex>("Sex");

    var result = from p in person.AsQueryable()
                    join s in sex.AsQueryable() on p.Sex equals s.Status
                    select new Person
                    {
                        Name = p.Name + "_" + s.SexName,
                        Sex = p.Sex,
                        Age = p.Age,
                        Birthday = p.Birthday,
                        ID = p.ID
                    };
    var persons = result.ToList().Take(10).ToList();
    return persons;
}
```



## 索引

创建索引

```c#
[HttpGet]
public List<string> CreateIndex()
{
    var keysDocument1 = new BsonDocument("ID", 1);
    var keysDocument2 = new BsonDocument("Name", 1);
    var keysDefinition1 = (IndexKeysDefinition<Person>)(keysDocument1);
    var keysDefinition2 = (IndexKeysDefinition<Person>)(keysDocument2);

    var model1 = new CreateIndexModel<Person>(keysDefinition1);
    var model2 = new CreateIndexModel<Person>(keysDefinition2);
    var response = _database.GetCollection<Person>(_collectionName).Indexes.CreateMany(new[] { model1, model2 }).ToList();

    return response;
}
```





# 参考

<https://github.com/mongodb/mongo-csharp-driver>

<https://austinsdev.com/article/2019/using-mongodb-asp-net-core-app>