using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Invocables
{
    public class SendPendingNotifications: IInvocable
    {
        public SendPendingNotifications()
        {
        }

        public Task Invoke()
        {
            return Task.CompletedTask;
        }
    }
}
