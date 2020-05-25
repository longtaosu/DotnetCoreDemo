using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queuing.Schedules
{
    public class BroadcastPayloadInvocable : IInvocable, IInvocableWithPayload<UserModel>
    {
        public UserModel Payload { get ; set ; }

        public Task Invoke()
        {
            Console.WriteLine($"接收到消息，姓名：{this.Payload.Name}，年龄：{this.Payload.Age}，性别：{this.Payload.Sex}");
            return Task.CompletedTask;
        }
    }

    public class UserModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Sex { get; set; }
    }
}
