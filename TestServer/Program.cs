using System.Net.Sockets;
using TestManagement;
using TestManagement.Entities;
using TestManagement.Services;

namespace TestServer
{
    public class Program {

        public static void Main(string[] args)
        {
            Console.WriteLine("SERVER APP");

            Server server = new Server();
            server.Start();
        }

    }
}