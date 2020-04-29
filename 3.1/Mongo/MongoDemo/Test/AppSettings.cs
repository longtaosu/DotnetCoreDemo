using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public class AppSettings
    {
        public MongoDb MongoDb { get; set; }
    }

    public class MongoDb
    {
        public string ConnectionString { get; set; }
        public string DbName { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
    }
}
