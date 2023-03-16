using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMToolbox.Services
{
    public class MissingServiceException : Exception
    {
        public MissingServiceException(string message) : base(message) {}
    }
}
