using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestClient.Services;

namespace TestClient
{
    public class Client
    {
        public static string SERVER_HOST = "127.0.0.1";
        public static int PORT = 11111;

        Socket client;

        EndPoint serverEndpoint;

        bool running = false;

        public Client()
        {
            IPAddress ipAddress = IPAddress.Parse(SERVER_HOST);

            client = new Socket(ipAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            serverEndpoint = (EndPoint)new IPEndPoint(ipAddress, PORT);

            // ClientService
            ClientService clientService = new ClientService();
            clientService.Start();
        }

        public void Start()
        {
            /*
            running = true;
            // test
            string message = "AUTHENTICATION\tadmin---admin";
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            int result = client.SendTo(buffer, buffer.Length, 0, serverEndpoint);
            if (result > 0) Console.WriteLine("Sent message to serevr.");
            else Console.WriteLine("ERROR: send message to server !!!");
            result = 0;
            buffer = new byte[1024 * 4];
            while (result == 0)
            {
                result = client.ReceiveFrom(buffer, buffer.Length, 0, ref serverEndpoint);
            }

            message = Encoding.UTF8.GetString(buffer, 0, result);
            Console.WriteLine(message);
            */

            Close();
        }

        public void Close()
        {
            client.Close();
        }
    }
}
