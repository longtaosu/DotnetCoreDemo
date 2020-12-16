using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficialDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficialDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        #region 初始化
        private string _collectionName = "Test";
        private IMongoDatabase _database;
        public SummaryController(IMongoDatabase database)
        {
            _database = database;
        }
        #endregion

        #region Collections
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<string> GetDbCollections()
        {
            var result = _database.ListCollectionNames().ToList();
            return result;
        }
        #endregion

        #region 添加数据
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
        #endregion

        #region 复杂查询
        [HttpGet]
        public List<Person> Complex(string name)
        {
            var result = _database.GetCollection<Person>(_collectionName).AsQueryable().Where(t => t.Name.Contains(name)).ToList();
            return result;
        }

        [HttpGet]
        public List<Person> Complex1(string name)
        {
            var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
            var result = _database.GetCollection<Person>(_collectionName).Find(filter).ToList();
            return result;
        }

        [HttpGet]
        public List<Person> Complex2(string name)
        {
            var result = _database.GetCollection<Person>(_collectionName).Find(t => t.Name.Contains(name)).ToList();
            return result;
        }
        #endregion

        #region 查询数量
        [HttpGet]
        public long CountDocuments(string name)
        {
            var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
            var result = _database.GetCollection<Person>(_collectionName).Find(filter).CountDocuments();

            return result;
        }

        [HttpGet]
        public long EstimatedDocumentCount()
        {
            var response = _database.GetCollection<Person>(_collectionName).EstimatedDocumentCount();//.Find(t => t.ID > 0).es
            return response;
        }
        #endregion

        #region 分页查询
        [HttpGet]
        public List<Person> QueryBySex(bool sex)
        {
            var result = _database.GetCollection<Person>(_collectionName).Find(t => t.Sex == true).Skip(100).Limit(10).ToList();
            return result;
        }
        #endregion

        #region 关联查询
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
        #endregion

        #region 排序
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<Person> QueryByOrder(bool sex)
        {
            var result = _database.GetCollection<Person>(_collectionName).Find(t => t.Sex == sex).SortBy(t => t.Name).Limit(10).ToList();
            return result;
        }
        #endregion

        #region 删除
        [HttpGet]
        public bool DeletePerson(int id)
        {
            var result = _database.GetCollection<Person>(_collectionName).DeleteOne(x => x.ID == id);
            return result.DeletedCount > 0;
        }

        [HttpGet]
        public bool DeleteMany(bool sex)
        {
            var result = _database.GetCollection<Person>(_collectionName).DeleteMany(x => x.Sex == sex);
            return result.DeletedCount > 0;
        }
        #endregion

        #region 索引
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

        [HttpGet]
        public Object GetIndexs()
        {

            try
            {
                return _database.GetCollection<Person>("Person").Indexes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 修改
        [HttpGet]
        public bool Update(int id)
        {
            var filter = Builders<Person>.Filter.Where(t => t.ID == id);
            var update = Builders<Person>.Update.Set(x => x.Sex, true);

            return _database.GetCollection<Person>(_collectionName).UpdateMany(filter, update).ModifiedCount > 0;
        }
        #endregion

    }
}
