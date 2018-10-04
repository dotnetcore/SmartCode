using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCode
{
    public class SmartCodeException : Exception
    {

        public SmartCodeException() { }
        public SmartCodeException(string message) : base(message) { }

        public SmartCodeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
