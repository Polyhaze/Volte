using SimpleServer;
using SimpleServer.Logging;
using System;
using System.Net;

namespace SIVA.Core.Server
{
    public class Server
    {
        public static SimpleServer.SimpleServer server;

        public static void Init()
        {
            SimpleServer.SimpleServer.Initialize(); // you only need to do this if you're logging
            Log.AddWriter(Console.Out);
            server = ServerBuilder.NewServer().NewHost(443).At(IPAddress.Any).With(new TestHandler()).AddToServer().BuildAndStart();
        }
    }
}
