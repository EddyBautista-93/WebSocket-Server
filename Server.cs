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
                while (!stream.DataAvailable) ;

                while (client.Available > 3) ;// wait for enough bytes to be available // Match against "get"


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

                    // http/1.1 defines the sequence CR LF as the end of the line marker
                    // CRLF refers to Carriage Return (ASCII 13, \r ) Line Feed (ASCII 10, \n )
                    byte[] response = Encoding.UTF8.GetBytes(
                                      "HTTP/1.1 101 Switching Protocols\r\n" +
                                      "Connection: Upgrade\r\n" +
                                      "Upgrade: websocket\r\n" +
                                      "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                    stream.Write(response, 0, response.Length);

                }
                else
                {
                    // must be true, "All messages from the client to the server have this bit set"
                    bool fin = (bytes[0] & 0b10000000) != 0,
                         mask = (bytes[1] & 0b10000000) != 0;

                    int opcode = bytes[0] & 0b00001111,
                                 offset = 2;
                    ulong msglen = (ulong)bytes[1] & 0b01111111;

                    if (msglen == 126)
                    {
                        // bytes are reversed because websockets will print them in Big-Endian, where as BitConverter will want them arranged in little endian on windows.
                        msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                        offset = 4;
                    }
                    else if (msglen == 127)
                    {
                        // To test the below code, we need to manually buffer larger messages - since the NIC's autobuffering may be too latency friendly for this code to run
                        // (this is, we may only have only some bytes in this websocket frame available through client.Available)
                        msglen = BitConverter.ToUInt64(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] }, 0);
                        offset = 10;
                    }

                    



                }

            }
        }
    }
}
