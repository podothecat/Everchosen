using System;
using System.Collections.Generic;
using System.Linq;

namespace EverChosenServer
{
    internal static class MatchingManager
    {
        // Clients wait matching.
        public static List<Client> Clients = new List<Client>();

        internal static Client MatchProcess(Client client)
        {
            if (Clients.Any())
            {
                var r = new Random();
                var idx = r.Next(0, Clients.Count);

                var opponent = Clients[idx];
                Clients.Remove(opponent);
                
                opponent.MatchingData.TeamColor = 1;
                client.MatchingData.TeamColor = 2;

                return opponent;
            }
            else
            {
                Clients.Add(client);
                return null;
            }
        }

        internal static void MatchCancelProcess(Client client)
        {
            Clients.Remove(client);
        }
    }
}