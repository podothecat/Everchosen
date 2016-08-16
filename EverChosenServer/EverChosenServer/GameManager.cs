using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace EverChosenServer
{
    internal static class GameManager
    {
        public static List<Client> Clients = new List<Client>();
        
        public static void AddClient(Client c)
        {
            Clients.Add(c);
            Console.WriteLine("\nGAME MANAGER : client was added.\n");
        }

        public static void ReleaseClient(Client c)
        {
            Clients.Remove(c);
            Console.WriteLine("\nGAME MANAGER : client was removed.\n");
        }

        public static void OnMatchingRequest(Client client)
        {
            MatchingManager.MatchProcess(client);
        }

        public static void OnIngameRequest(Client client)
        {
            // Write code for game logic.
        }
    }
}
