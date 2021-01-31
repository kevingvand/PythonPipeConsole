using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonPipeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("XXX");
            server.Run();
        }
    }
}
