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
    public class MongoIndexController : ControllerBase
    {
        private IMongoDatabase _database;
        public MongoIndexController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpGet]
        public List<BsonDocument> GetIndexs()
        {
            var result = _database.GetCollection<Person>("LargePerson").Indexes.List().ToList();
            return result;
        }
    }
}
