using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPubSub.Services
{
    public interface IService
    {
        Task ServiceTheThing(String value);
    }
}
