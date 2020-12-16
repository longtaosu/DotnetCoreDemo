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
    public class RelateQueryController : ControllerBase
    {
        private IMongoDatabase _database;

        public RelateQueryController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpGet]
        public void InitData()
        {
            List<Sex> lstData = new List<Sex>();
            lstData.Add(new Sex
            {
                ID = 1,
                SexName = "男",
                Status = true
            });
            lstData.Add(new Sex
            {
                ID = 2,
                Status = false,
                SexName = "女"
            });

            _database.GetCollection<Sex>("Sex").InsertMany(lstData);

        }

        [HttpGet]
        public List<string> CreateIndex()
        {
            var keysDocument1 = new BsonDocument("ID", 1);
            var keysDocument2 = new BsonDocument("Name", 1);
            var keysDefinition1 = (IndexKeysDefinition<Person>)(keysDocument1);
            var keysDefinition2 = (IndexKeysDefinition<Person>)(keysDocument2);

            var model1 = new CreateIndexModel<Person>(keysDefinition1);
            var model2 = new CreateIndexModel<Person>(keysDefinition2);
            var response = _database.GetCollection<Person>("Person").Indexes.CreateMany(new[] { model1, model2 }).ToList();


            var keysDocument3 = new BsonDocument("ID", 1);
            var keysDocument4 = new BsonDocument("Status", 1);
            var keysDefinition3 = (IndexKeysDefinition<Person>)(keysDocument3);
            var keysDefinition4 = (IndexKeysDefinition<Person>)(keysDocument4);

            var model3 = new CreateIndexModel<Person>(keysDefinition3);
            var model4 = new CreateIndexModel<Person>(keysDefinition4);
            response = _database.GetCollection<Person>("Sex").Indexes.CreateMany(new[] { model3, model4 }).ToList();

            return response;
        }


        [HttpGet]
        public List<Person> RelateQuery()
        {
            var person = _database.GetCollection<Person>("Person");
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

        [HttpGet]
        public List<Person> Query()
        {
            return _database.GetCollection<Person>("Person").Find(t => true).Limit(10).ToList();
        }
    }

    public class Sex
    {
        public int ID { get; set; }

        public bool Status { get; set; }

        public string SexName { get; set; }
    }
}
