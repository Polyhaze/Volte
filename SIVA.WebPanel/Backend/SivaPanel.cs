using System;
using SimpleServer;
using SimpleServer.Logging;
using System.Net;

namespace SIVA.WebPanel.Backend
{
    public class SivaPanel
    {
        public static SimpleServer.SimpleServer server;
        public static void InitialiseServer()
        {
            SimpleServer.SimpleServer.Initialize();
            Log.AddWriter(Console.Out);
            server = ServerBuilder.NewServer()
                .NewHost(443)
                .At(IPAddress.Any)
                .With(new Handler())
                .AddToServer()
                .BuildAndStart();

        }
    }
}
