using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficialDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// https://digitteck.com/mongo-csharp/filters-in-mongo-csharp/
/// </summary>
namespace OfficialDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ComplexQueryController : ControllerBase
    {
        private IMongoDatabase _database;
        public ComplexQueryController(IMongoDatabase database)
        {
            _database = database;
        }

        #region Database
        [HttpGet]
        public List<string> GetDbCollections()
        {
            var result = _database.ListCollectionNames().ToList();//.ListCollections().ToList();
            return result;
        }
        #endregion

        #region 复杂查询
        [HttpGet]
        public List<Person> Complex(string name)
        {
            var result = _database.GetCollection<Person>("Person").AsQueryable().Where(t => t.Name.Contains(name)).ToList();
            return result;
        }

        [HttpGet]
        public List<Person> Complex1(string name)
        {
            var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
            var result = _database.GetCollection<Person>("Person").Find(filter).ToList();
            return result;
        }

        [HttpGet]
        public List<Person> Complex2(string name)
        {
            //var filter = Builders<Person>.Filter.Where(t => t.Name.Contains(name));
            var result = _database.GetCollection<Person>("Person").Find(t => t.Name.Contains(name)).ToList();
            return result;
        }
        #endregion
    }
}
