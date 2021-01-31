using PythonPipeServer.Enumerations;
using PythonPipeServer.Messages;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace PythonPipeServer
{
    public class Server
    {
        private NamedPipeServerStream _server;
        private BinaryReader _reader;
        private BinaryWriter _writer;

        public string Name { get; set; }

        public Server(string name)
        {
            Name = name;
        }

        public void Run()
        {
            _server = new NamedPipeServerStream(Name);

            LogService.LogInfo("Waiting for connection...");
            _server.WaitForConnection();

            LogService.LogInfo("Client Connected");

            _reader = new BinaryReader(_server);
            _writer = new BinaryWriter(_server);

            while (true)
            {
                LogService.LogInfo("Enter code to execute:");
                var input = Console.ReadLine();
                try
                {
                    WriteMessage(input);

                    var result = ReadMessage();

                    if (result == null)
                    {
                        LogService.LogError("Invalid response, terminating...");
                        return;
                    }

                    if (result.MessageType.Equals(EMessageType.ERROR))
                        LogService.LogError(((TextMessage)result).Text);
                    else
                    {
                        var resultMessage = (ResultMessage)result;
                        var value = resultMessage.GetValue();
                        LogService.LogInfo($"{value} ({resultMessage.Type})");
                    }

                }
                catch (IOException)
                {
                    _server.Close();
                    LogService.LogError("Connection lost, reconnecting.");
                    Run();
                    return;
                }

            }
        }

        private void WriteMessage(string input)
        {
            BaseMessage message;

            if (input.StartsWith("eval"))
            {
                var text = input.Substring(5);
                message = new TextMessage(EMessageType.EVALUATE, text);
            }
            else message = new TextMessage(EMessageType.EXECUTE, input);

            _writer.Write(message.GetBytes());
        }

        private BaseMessage ReadMessage()
        {
            var messageType = (EMessageType)_reader.ReadByte();
            if (messageType == EMessageType.ERROR)
            {
                var length = _reader.ReadByte() * 256 + _reader.ReadByte();
                var errorMessage = Encoding.ASCII.GetString(_reader.ReadBytes(length));

                return new TextMessage(EMessageType.ERROR, errorMessage);
            }
            else if (messageType == EMessageType.RESULT)
            {
                var resultType = (EType)_reader.ReadByte();
                var length = _reader.ReadByte() * 256 + _reader.ReadByte();

                return new ResultMessage(resultType, _reader.ReadBytes(length));
            }
            else
            {
                return null;
            }
        }
    }
}
