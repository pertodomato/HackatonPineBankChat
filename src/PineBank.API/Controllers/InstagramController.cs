// PineBank/src/PineBank.API/Controllers/InstagramController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using PineBank.API.Models;
using PineBank.Application.Interfaces;
using System.Linq;

namespace PineBank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstagramController : ControllerBase
    {
        private readonly ILogger<InstagramController> _logger;
        private readonly IChatService _chatService;
        private readonly IInstagramService _instagramService; // Service to handle Instagram responses

        public InstagramController(ILogger<InstagramController> logger, IChatService chatService, IInstagramService instagramService)
        {
            _logger = logger;
            _chatService = chatService;
            _instagramService = instagramService;
        }

        // GET endpoint for webhook verification
        [HttpGet("webhook")]
        public IActionResult VerifyWebhook([FromQuery] string hub_mode, [FromQuery] string hub_challenge, [FromQuery] string hub_verify_token)
        {
            const string VERIFY_TOKEN = "YOUR_VERIFY_TOKEN"; // Replace with your actual verify token

            if (hub_mode == "subscribe" && hub_verify_token == VERIFY_TOKEN)
            {
                _logger.LogInformation("Webhook verified successfully.");
                return Content(hub_challenge);
            }

            _logger.LogWarning("Webhook verification failed.");
            return Unauthorized();
        }

        // POST endpoint to receive messages
        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveMessage([FromBody] InstagramWebhookRequest request)
        {
            if (request.Object != "instagram")
            {
                _logger.LogWarning("Received unsupported object type: {Object}", request.Object);
                return Ok();
            }

            foreach (var entry in request.Entry)
            {
                foreach (var messaging in entry.Messaging)
                {
                    var senderId = messaging.Sender.Id;
                    var messageText = messaging.Message?.Text;

                    if (!string.IsNullOrEmpty(messageText))
                    {
                        _logger.LogInformation("Received message from {SenderId}: {MessageText}", senderId, messageText);

                        // Process the message using the chat service
                        var response = await _chatService.HandleMessageAsync(senderId, messageText);

                        // Send the response back to Instagram
                        await _instagramService.SendMessageAsync(senderId, response.Message);
                    }
                    else
                    {
                        _logger.LogInformation("Received non-text message from {SenderId}.", senderId);
                        // Handle other message types if necessary
                    }
                }
            }

            return Ok();
        }
    }
}
