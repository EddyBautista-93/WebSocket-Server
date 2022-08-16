using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace WebSocket_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 80;
            TcpListener server = new TcpListener(IPAddress.Parse(ip), port); // Listens for connections from Tcp network clients.
            
            server.Start();
            Console.WriteLine("Server has started on {0}:{1}, Waiting for a connection…", ip, port);

            TcpClient client = server.AcceptTcpClient(); // Waits for a tcp connection, accepts it and returns it as a TcpClient Object
            Console.WriteLine("A client connected");

            NetworkStream stream = client.GetStream(); // gets the stream which is the communication channel , both sides of the channel have read and write capabilities.

            // enter to a infinite cycle to be able to handle every change in stream
            while (true)
            {
                while (!stream.DataAvailable);
                
                    while (client.Available > 3);// wait for enough bytes to be available // Match against "get"
                    

                    Byte[] bytes = new Byte[client.Available];
                    stream.Read(bytes, 0, bytes.Length); // params(reads bytes to buffer, offsite, size) last two determine the length of the message.
                    string data = Encoding.UTF8.GetString(bytes); // translates byes of the request to a string

                    if (Regex.IsMatch(data, "^GET", RegexOptions.IgnoreCase))
                    {
                        Console.WriteLine("=====Handshaking from client=====\n{0}", data);

                        // 1. Obtain the values of the websocket key request header without leading or trailing white whitespace.
                        string swk = Regex.Match(data, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();

                         // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                        string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

                        // 3. Compute SHA-1 and Base64 hash of the new value.
                        byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));

                        // 4. Write the has back as the value of "Sec-WebSocket-Accept" Response header in an http response.
                        string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                    } else {
                        
                    }
                
            }
        }
    }
}
