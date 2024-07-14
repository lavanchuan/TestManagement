using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Requests
{
    public class ClientRequest
    {
        public string type = "";
        public string msg = "";

        public static string AUTHENTICATION = "AUTHENTICATION";
        public static string REGISTER = "REGISTER";

        public ClientRequest(string message)
        {
            Console.WriteLine(message);
            try
            {
                type = message.Split(FormatService.PATTERN)[0];
                msg = message.Split(FormatService.PATTERN)[1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: failed extract ClientRequest !!!");
            }
        }
    }
}
