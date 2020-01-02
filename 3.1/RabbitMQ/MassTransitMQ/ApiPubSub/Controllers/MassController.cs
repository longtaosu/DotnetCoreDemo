using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiPubSub.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPubSub.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MassController : ControllerBase
    {
        private readonly IRequestClient<DoSomething> _requestClient;
        public MassController(IRequestClient<DoSomething> requestClient)
        {
            _requestClient = requestClient;
        }


        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            try
            {
                var request = _requestClient.Create(new { Value = $"SendTime:{DateTime.Now},Hello, World." }, cancellationToken);

                var response = await request.GetResponse<SomethingDone>();

                return Content($"{response.Message.Value}, MessageId: {response.MessageId:D}");
            }
            catch (RequestTimeoutException exception)
            {
                return StatusCode((int)HttpStatusCode.RequestTimeout);
            }
        }
    }
}