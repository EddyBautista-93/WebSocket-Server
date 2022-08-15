using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net;

namespace WebSocket_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection…", Environment.NewLine);
            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("A client connected");
        }
    }
}
