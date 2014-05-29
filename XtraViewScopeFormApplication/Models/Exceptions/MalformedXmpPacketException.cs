using System;

namespace XtraViewScopeFormApplication.Models.Exceptions
{
    class MalformedXmpPacketException : Exception
    {
        public MalformedXmpPacketException() { }

        public MalformedXmpPacketException(string message) : base(message) { }
    }
}
