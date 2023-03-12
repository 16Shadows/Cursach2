using System;

namespace DMOrganizerModel.Implementation.Utility
{
    /// <summary>
    /// Is thrown when an argument's type does not match the expected type (for instance, passing custom implementation of an interface to the model).
    /// </summary>
    public sealed class ArgumentTypeException : ArgumentException
    {
        public ArgumentTypeException() : base() {}
        public ArgumentTypeException(string param) : base("", param) {}
        public ArgumentTypeException(string param, string message) : base(message, param) {}
    }
}
