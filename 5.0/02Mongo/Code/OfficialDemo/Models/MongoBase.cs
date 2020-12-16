using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficialDemo.Models
{
    public class MongoBase
    {
        public ObjectId _id { get; set; }
    }
}
