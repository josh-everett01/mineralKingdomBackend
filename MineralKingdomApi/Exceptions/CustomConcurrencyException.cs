using System;
namespace MineralKingdomApi.Exceptions
{
    public class CustomConcurrencyException : Exception
    {
        public CustomConcurrencyException()
        {
        }

        public CustomConcurrencyException(string message)
            : base(message)
        {
        }

        public CustomConcurrencyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}