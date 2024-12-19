// PineBank/src/PineBank.Infrastructure/Services/InstagramService.cs
using PineBank.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PineBank.Infrastructure.Services
{
    public class InstagramService : IInstagramService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<InstagramService> _logger;
        private readonly HttpClient _httpClient;

        public InstagramService(IConfiguration config, ILogger<InstagramService> logger)
        {
            _config = config;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task SendMessageAsync(string recipientId, string messageText)
        {
            var accessToken = _config["IG_PAGE_ACCESS_TOKEN"];
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("Instagram Page Access Token is not configured.");
                return;
            }

            var url = $"https://graph.facebook.com/v17.0/me/messages?access_token={accessToken}";

            var payload = new
            {
                recipient = new { id = recipientId },
                message = new { text = messageText }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Message sent successfully to {RecipientId}.", recipientId);
                }
                else
                {
                    _logger.LogError("Failed to send message to {RecipientId}. Response: {Response}", recipientId, responseString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while sending message to {RecipientId}.", recipientId);
            }
        }
    }
}
