//./tests/PineBank.Tests/ChatbotTests.cs


using Xunit;
using PineBank.Application.Interfaces;
using PineBank.Application.DTOs;
using Moq;
using PineBank.Domain.Interfaces;

namespace PineBank.Tests
{
    public class ChatbotTests
    {
        [Fact]
        public async Task HandleMessageAsync_ReturnsDummyResponse()
        {
            // Arrange
            var mockUow = new Mock<IUnitOfWork>();
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<PineBank.Infrastructure.Services.ChatService>>();

            mockConfig.Setup(c => c["LLM_PROVIDER"]).Returns("openai");
            mockConfig.Setup(c => c["CURRENCY_API_BASE"]).Returns("https://economia.awesomeapi.com.br");

            var chatService = new PineBank.Infrastructure.Services.ChatService(mockUow.Object, mockConfig.Object, mockLogger.Object);

            // Act
            var response = await chatService.HandleMessageAsync("test-user", "What is the exchange rate?");

            // Assert
            Assert.NotNull(response);
            Assert.False(response.NeedsEmployee);
            Assert.Equal("This is a response from OpenAI GPT.", response.Message);
        }
    }
}
