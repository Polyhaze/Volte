using System.IO;
using SimpleServer.Handlers;
using SimpleServer.Internals;

namespace SIVA.WebServer {
    public class WebHandler : IHandler {
        public bool CanHandle(SimpleServerRequest req) {
            return req.FormattedUrl.Equals("/");
        }

        public void Handle(SimpleServerContext ctx) {
            if (!ctx.Request.FormattedUrl.Equals("/")) return;
            var sw = new StreamWriter(ctx.Response.OutputStream);
            sw.WriteLine("<h1>Greetings</h1>");
            sw.Flush();
            sw.Close();
            ctx.Response.Close();
        }
    }
}