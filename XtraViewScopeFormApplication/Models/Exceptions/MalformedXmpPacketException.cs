using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtraViewScope.Models.Exceptions
{
    class MalformedXmpPacketException : Exception
    {
        public MalformedXmpPacketException() { }

        public MalformedXmpPacketException(string message) : base(message) { }
    }
}
