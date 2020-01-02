using MassTransit.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPubSub.Services
{
    public class Service : IService
    {
        public Task ServiceTheThing(string value)
        {
            return TaskUtil.Completed;
        }
    }
}
