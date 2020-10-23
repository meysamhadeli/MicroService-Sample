using System;

namespace MicroPack.MicroPack
{
    public class MicroPackException : Exception
    {
        public string Code { get; }

        public MicroPackException()
        {
        }

        public MicroPackException(string code)
        {
            Code = code;
        }

        public MicroPackException(string message, params object[] args) 
            : this(string.Empty, message, args)
        {
        }

        public MicroPackException(string code, string message, params object[] args) 
            : this(null, code, message, args)
        {
        }

        public MicroPackException(Exception innerException, string message, params object[] args)
            : this(innerException, string.Empty, message, args)
        {
        }

        public MicroPackException(Exception innerException, string code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }        
    }
}