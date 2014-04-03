using System;

namespace XtraViewScopeFormApplication.Models.Exceptions
{
    public class ScopeNotInitialisedException : Exception
    {
        public ScopeNotInitialisedException()
        {
        }

        public ScopeNotInitialisedException(string message)
            : base(message)
        {
        }
    }
}
