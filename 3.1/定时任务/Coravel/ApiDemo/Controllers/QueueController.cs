using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Invocables;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private IQueue _queue;

        public QueueController(IQueue queue)
        {
            this._queue = queue;
        }

        public IActionResult TriggerExpensiveStuff()
        {
            this._queue.QueueInvocable<DoExpensiveCalculationAndStore>();
            return Ok();
        }
    }
}