using System;
using System.Net;
using SimpleServer;
using SimpleServer.Logging;

namespace SIVA.WebServer {
    public class WebServer {
        private const int WebPort = 4200;
        public void Start() {
            SimpleServer.SimpleServer.Initialize();
            Log.AddWriter(Console.Out);
            var server = ServerBuilder //the server variable isnt used yet, but lets just add it
                .NewServer()
                .NewHost(WebPort)
                .At(IPAddress.Loopback)
                .With(new WebHandler())
                .AddToServer()
                .BuildAndStart();
            new Core.Runtime.Log().Info($"Web Server started on port {WebPort}.");
        }
    }
}