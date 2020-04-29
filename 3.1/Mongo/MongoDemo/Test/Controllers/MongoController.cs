using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MongoController : ControllerBase
    {
        private IMongoClient _mongoClient;
        public MongoController(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        [HttpGet]
        public bool AddInfo(string name,int age,bool sex)
        {
            var collection = _mongoClient.GetDatabase("Test").GetCollection<Person>("Person");
            try
            {
                collection.InsertOne(new Person
                {
                    _id = (object)DateTime.Now,
                    Age = age,
                    Name = name,
                    Sex = sex
                });
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }


        [HttpGet]
        public List<Person> GetAllPerson()
        {
            var collection = _mongoClient.GetDatabase("Test").GetCollection<Person>("Person");
            return collection.Find(x => true).ToList();
        }


    [HttpGet]
    public bool DeleteInfo(string name)
    {
        var collection = _mongoClient.GetDatabase("Test").GetCollection<Person>("Person");
        try
        {
            collection.DeleteOne(x => x.Name == name);
            return true;
        }
        catch(Exception ex)
        {
            return false;
        }
    }
    }

    public class Person
    {
        public object _id { get; set; }
        public int Age { get; set; }

        public string Name { get; set; }

        public bool Sex { get; set; }
    }
}