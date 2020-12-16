using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficialDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OfficialDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MongoController : ControllerBase
    {
        private IMongoDatabase _database;
        public MongoController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpGet]
        public TimeSpan InitData()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Person> lstData = new List<Person>();
            for (int i = 0; i < 10000; i++)
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
                _database.GetCollection<Person>("LargePerson").InsertMany(lstData);
            }
            //for (int i = 0; i < 10000 * 10000; i++)
            //{
            //    var person = new Person
            //    {
            //        ID = i,
            //        Age = i,
            //        Name = $"name_{i}",
            //        Birthday = DateTime.Now.AddSeconds(i),
            //        Sex = i % 2 == 0
            //    };
            //    lstData.Add(person);
            //}

            //_database.GetCollection<Person>("LargePerson").InsertMany(lstData);

            sw.Stop();
            return sw.Elapsed;
        }


        [HttpGet]
        public List<Person> GetAllPersons()
        {
            var result = _database.GetCollection<Person>("Person").Find(t=>true).ToList();
            return result;
        }

        [HttpGet]
        public List<Person> GetPersonByName(string name)
        {
            //var result = _database.GetCollection<Person>("LargePerson").Find(t => t.Name.Contains(name)).ToList();

            var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
            var result = _database.GetCollection<Person>("LargePerson").Find(filter).ToList();

            //var result = _database.GetCollection<Person>("Person").Find({Name:/^name/});

            return result;
        }

        [HttpGet]
        public long GetPersonByNameCount(string name)
        {
            //var result = _database.GetCollection<Person>("LargePerson").Find(t => t.Name.Contains(name)).ToList();

            var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
            var result = _database.GetCollection<Person>("Person").Find(filter).CountDocuments();//.ToList().cou;

            //var result = _database.GetCollection<Person>("Person").Find({Name:/^name/});

            return result;
        }

        [HttpGet]
        public List<Person> GetOnlyPersonByName(string name)
        {
            var result = _database.GetCollection<Person>("LargePerson").Find(t => t.Name == name).ToList();
            return result;
        }

        [HttpGet]
        public long GetPersonsCount()
        {
            var response = _database.GetCollection<Person>("LargePerson").EstimatedDocumentCount();//.Find(t => t.ID > 0).es
            return response;
        }

        [HttpPost]
        public List<Person> AddPerson(Person person)
        {
            try
            {
                _database.GetCollection<Person>("Person").InsertOne(person);
                return _database.GetCollection<Person>("Person").Find(t => true).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public bool DeletePerson(int id)
        {
            var result = _database.GetCollection<Person>("Person").DeleteOne(x => x.ID == id);
            return result.DeletedCount > 0;
        }

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
            var response = _database.GetCollection<Person>("LargePerson").Indexes.CreateMany(new[] { model1, model2 }).ToList();

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

        #region 复杂查询
        [HttpGet]
        public List<Person> ComplexQuery(string name)
        {
            var builderFilter = Builders<Person>.Filter;
            FilterDefinition<Person> filter = builderFilter.Regex("Name", new BsonRegularExpression(name));

            var result = _database.GetCollection<Person>("LargePerson").Find(filter).ToList();
            return result;
        }



        [HttpGet]
        public void AggregateQuery()
        {
            //_database.GetCollection<Person>("LargePerson").Aggregate()
            //LookupOperation
        }
        #endregion

        [HttpGet]
        public List<Person> QueryBySex(bool sex)
        {
            var result = _database.GetCollection<Person>("LargePerson").Find(t=>t.Sex==true).Skip(100).Limit(10).ToList();
            return result;
        }

        //2020-11-24T04:03:09.116Z
        [HttpGet]
        public List<Person> QueryByBirthdat()
        {
            var result = _database.GetCollection<Person>("LargePerson").Find(t =>t.Birthday>new DateTime(2020,11,24,04,03,09)).Limit(10).ToList();
            return result;
        }

        [HttpGet]
        public List<Person> QueryByOrder()
        {
            var result = _database.GetCollection<Person>("LargePerson").Find(t =>true).SortBy(t=>t.Name).Limit(10).ToList();
            return result;
        }
    }
}
