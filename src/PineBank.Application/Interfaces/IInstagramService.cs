// PineBank/src/PineBank.Application/Interfaces/IInstagramService.cs
namespace PineBank.Application.Interfaces
{
    public interface IInstagramService
    {
        Task SendMessageAsync(string recipientId, string messageText);
    }
}
