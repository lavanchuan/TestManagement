using TestClient.Services;
using TestManagement;

namespace TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("CLIENT APP");

            //Client client = new Client();
            //client.Start();

            ClientService service = new ClientService();
            service.Start();
        }
    }
}