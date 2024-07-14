using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Requests;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TestClient.Services
{
    public class Authentication : WebSocketBehavior
    {
        Socket client;
        EndPoint serverEndpoint;

        const string SERVER_HOST = "127.0.0.1";
        const int PORT = 11111;

        public Authentication() {
            IPAddress ipAddress = IPAddress.Parse(SERVER_HOST);

            client = new Socket(ipAddress.AddressFamily,
                SocketType.Dgram,
                ProtocolType.Udp);

            serverEndpoint = (EndPoint)new IPEndPoint(ipAddress, PORT);
        }

        protected override void OnMessage(MessageEventArgs e) { 
            base.OnMessage(e);

            string message = e.Data;

            Thread thread = new Thread(() => SendRequestAndRecvResponse(message));
            thread.Start();

            Console.WriteLine(message);
        }

        private void SendRequestAndRecvResponse(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            int result = client.SendTo(buffer, buffer.Length, 0, serverEndpoint);
            if (result == 0)
            {
                Console.WriteLine("ERROR: send request to server !!!");
                return;
            }

            result = 0;
            buffer = new byte[1024 * 4];
            while (result == 0) {
                result = client.ReceiveFrom(buffer, 1024 * 4, 0, ref serverEndpoint);
            }

            Send(Encoding.UTF8.GetString(buffer, 0, result));
        }

        protected override void OnOpen()
        {
            Console.WriteLine($"Client service is opened.");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine(e.Reason);
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine($"ERROR: {e.Message} !!!");
        }

    }
}
