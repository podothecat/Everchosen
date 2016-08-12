using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            Console.WriteLine("GAME MANAGER : client was added.");
        }

        public static void ReleaseClient(Client c)
        {
            Clients.Remove(c);
            Console.WriteLine("GAME MANAGER : client was removed.");
        }

        public static void OnMatchingRequest(Client client)
        {
            MatchingManager.MatchProcess(client);
        }
    }

    internal static class MatchingManager
    {
        public static Queue<Client> Clients = new Queue<Client>();

        internal static void MatchProcess(Client client)
        {
            if (Clients.Any())
            {
                var opponent = Clients.Dequeue();

                opponent.MatchingData.TeamColor = 1;
                client.MatchingData.TeamColor = 2;
                opponent.SendPacket("OnSucceedMatching", client);
                client.SendPacket("OnSucceedMatching", opponent);
            }
            else
            {
                Clients.Enqueue(client);
            }
        }
    }
}
