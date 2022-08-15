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
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80); // Listens for connections from Tcp network clients.
            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection…", Environment.NewLine);
            TcpClient client = server.AcceptTcpClient(); // Waits for a tcp connection, accepts it and returns it as a TcpClient Object

            Console.WriteLine("A client connected");

            NetworkStream stream = client.GetStream(); // gets the stream which is the communication channel , both sides of the channel have read and write capabilitys.

            // enter to a infinite cycle to be able to handle every change in stream
            while (true)
            {
                while (!stream.DataAvailable)
                {
                    Byte[] bytes = new Byte[client.Available];

                    stream.Read(bytes, 0, bytes.Length); // params(reads bytes to buffer, offsite, size) last two determine the length of the message.
                }
            }
        }
    }
}
