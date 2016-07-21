using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MatchingServer server = new MatchingServer();

            server.StartServer(8889);

            while (true)
            {

            }
        }
    }
}
