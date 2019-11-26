using System;

namespace FabrikamFiber.Web.Controllers
{
    public class ConnectionTimeoutException : Exception
    {
        public ConnectionTimeoutException()
        {
        }

        public ConnectionTimeoutException(string message)
            : base(message)
        {
        }

        public ConnectionTimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}