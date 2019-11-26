using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutofacDemo.AspNetCoreDemo.Services
{
    public interface IValuesService
    {
        IEnumerable<string> FindAll();

        string Find(int id);
    }
}
