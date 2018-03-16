using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AutoMailbox.Controllers
{
    [Produces("application/json")]
    [Route("api/email")]
    public class EmailController : Controller
    {
        private readonly EmailQueue _emailQueue;
        private readonly ILogger<EmailController> _logger;

        public EmailController(EmailQueue emailQueue, ILogger<EmailController> logger)
        {
            _emailQueue = emailQueue;
            _logger = logger;
        }

        [HttpGet]
        [Route("{username}")]
        [ProducesResponseType(typeof(Email), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public IActionResult Get(string username)
        {
            _logger.LogInformation("Retrieving last email for {Username}.", username);
            if (_emailQueue.TryGetLatest(username, out var email))
                return new ObjectResult(email);

            return NotFound();
        }

        [HttpPost]
        public IActionResult Post(EmailFormModel postedEmail)
        {
            _logger.LogInformation("Incoming email: {@Email}", postedEmail);

            var email = Email.Parse(postedEmail);
            _logger.LogInformation("Parsed email: {@Email}", email);

            _emailQueue.Enqueue(email);

            return Ok();
        }
    }
}