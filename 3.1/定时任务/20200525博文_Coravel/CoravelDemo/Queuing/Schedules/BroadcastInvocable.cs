using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queuing.Schedules
{
    public class BroadcastInvocable : IInvocable
    {
        public async Task Invoke()
        {
            Console.WriteLine($"处理一个耗时操作，耗时15s！时间：{DateTime.Now}");
            await Task.Delay(15000);
            Console.WriteLine($"耗时操作处理完成，时间：{DateTime.Now}");
        }
    }
}
