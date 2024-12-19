// ./src/PineBank.Application/Interfaces/IChatService.cs
using PineBank.Application.DTOs;
using System.Threading.Tasks;

namespace PineBank.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatbotResponseDto> HandleMessageAsync(string userId, string userMessage);
    }
}
