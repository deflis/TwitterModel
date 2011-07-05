using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NekoVampire.TwitterCore
{
    internal class NoAuthenticationException : Exception
    {
        public NoAuthenticationException()
            : base("Twitter認証がなされていません。") { }

        public NoAuthenticationException(Exception innerException)
            : base("Twitter認証がなされていません。", innerException) { }

        public NoAuthenticationException(string message)
            : base(message) { }

        public NoAuthenticationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
