using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtraViewScope.ConnectionManagement
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
