// ./src/PineBank.API/Models/LoginRequest.cs
namespace PineBank.API.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
