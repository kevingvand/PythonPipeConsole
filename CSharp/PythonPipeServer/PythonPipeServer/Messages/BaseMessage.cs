using PythonPipeServer.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonPipeServer.Messages
{
    public abstract class BaseMessage
    {
        public EMessageType MessageType { get; set; }

        public abstract byte[] GetBytes();
    }
}
