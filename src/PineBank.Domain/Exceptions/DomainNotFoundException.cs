// ./src/PineBank.Domain/Exceptions/DomainNotFoundException.cs

using System;

namespace PineBank.Domain.Exceptions
{
    public class DomainNotFoundException : Exception
    {
        public DomainNotFoundException() : base() { }

        public DomainNotFoundException(string message) : base(message) { }

        public DomainNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
