// ./src/PineBank.Application/Services/ChatService.cs
using PineBank.Application.Interfaces;
using PineBank.Application.DTOs;

namespace PineBank.Application.Services
{
    public class ChatService : IChatService
    {
        public async Task<ChatbotResponseDto> HandleMessageAsync(string userId, string userMessage)
        {
            // For now, return a simple response
            return new ChatbotResponseDto
            {
                Message = "This is a test response from the chatbot",
                Confidence = 1.0,
                NeedsEmployee = false,
                Metadata = new ChatbotResponseMetadataDto
                {
                    Intent = "test_intent",
                    Topic = "test_topic"
                }
            };
        }
    }
}