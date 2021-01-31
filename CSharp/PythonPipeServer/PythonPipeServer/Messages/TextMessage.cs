using PythonPipeServer.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonPipeServer.Messages
{
    public class TextMessage : BaseMessage
    {
        public string Text { get; set; }

        public TextMessage(EMessageType messageType, string text)
        {
            MessageType = messageType;
            Text = text;
        }

        public override byte[] GetBytes()
        {
            var text = Encoding.ASCII.GetBytes(Text);

            byte[] result = new byte[text.Length + 3];
            result[0] = (byte)MessageType;
            result[1] = (byte)(text.Length / 256);
            result[2] = (byte)(text.Length & 255);
            text.CopyTo(result, 3);

            return result;
        }
    }
}
