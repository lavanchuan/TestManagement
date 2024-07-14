using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace TestClient.Services
{
    public class ClientService
    {
        public ClientService() { }

        public void Start() {
            Thread authenticationThread = new Thread(() =>
            {
                var webSocket = new WebSocketServer($"ws://{Client.SERVER_HOST}:" +
                    $"{Client.PORT}");

                webSocket.AddWebSocketService<Authentication>("/authentication");

                webSocket.Start();

                Console.ReadLine();

                webSocket.Stop();
            });

            authenticationThread.Start();
        }
    }
}
