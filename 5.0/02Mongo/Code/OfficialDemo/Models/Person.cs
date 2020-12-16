using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficialDemo.Models
{
    public class Person : MongoBase
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime Birthday { get; set; }

        public bool Sex { get; set; }
    }
}
