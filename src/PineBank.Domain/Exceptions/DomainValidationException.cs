//./src/PineBank.Domain/Exceptions/DomainValidationException.cs
namespace PineBank.Domain.Exceptions
{
    public class DomainValidationException : Exception
    {
        public DomainValidationException(string message) : base(message) { }
    }
}
