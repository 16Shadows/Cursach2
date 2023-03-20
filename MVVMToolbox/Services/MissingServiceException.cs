using System;

namespace MVVMToolbox.Services
{
    public class MissingServiceException : Exception
    {
        public MissingServiceException(string message) : base(message) {}
    }
}
