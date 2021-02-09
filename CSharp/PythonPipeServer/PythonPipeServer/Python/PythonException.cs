using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PythonPipeServer.Python
{
    [Serializable]
    internal class PythonException : Exception
    {
        public PythonException()
        {
        }

        public PythonException(string message) : base(message)
        {
        }

        public PythonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PythonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
