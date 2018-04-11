using SimpleServer.Handlers;
using SimpleServer.Internals;
using SimpleServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class TestHandler : IHandler
{
    public bool CanHandle(SimpleServerRequest request)
    {
        if (request.FormattedUrl == "/")
        {
            return true;
        }
        return false;
    }
    public void Handle(SimpleServerContext context)
    {
        if (context.Request.FormattedUrl == "/")
        {
            StreamWriter sw = new StreamWriter(context.Response.OutputStream);
            sw.WriteLine("Hello!");
            sw.Close();
            context.Response.Close();
        }
    }
}