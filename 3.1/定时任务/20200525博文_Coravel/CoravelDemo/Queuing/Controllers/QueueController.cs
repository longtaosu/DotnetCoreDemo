using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Queuing.Events;
using Queuing.Schedules;

namespace Queuing.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        IQueue _queue;
        public QueueController(IQueue queue)
        {
            this._queue = queue;
        }

        public IActionResult TriggerBroadcast()
        {
            _queue.QueueInvocable<BroadcastInvocable>();
            return Ok();
        }

        public IActionResult TriggerPayloadBroadcast()
        {
            var user = new UserModel()
            {
                Name = "张三",
                Age = DateTime.Now.Second,
                Sex = new Random().Next(1, 3) % 2 == 0 ? true : false
            };
            _queue.QueueInvocableWithPayload<BroadcastPayloadInvocable, UserModel>(user);
            return Ok();
        }

        public IActionResult TriggerAsyncTask()
        {
            _queue.QueueAsyncTask(async () =>
            {
                await Task.Delay(1000);
                Console.WriteLine("this was queued, async");
            });
            return Ok();
        }

        public IActionResult TriggerSyncTask()
        {
            _queue.QueueTask(() =>
            {
                Console.WriteLine("this was queued, sync");
            });
            return Ok();
        }


        public IActionResult TriggerBroadcastEvent()
        {
            _queue.QueueBroadcast(new DemoEvent(DateTime.Now.ToString()));
            return Ok();
        }

    }
}