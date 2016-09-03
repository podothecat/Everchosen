using System;
using System.Collections.Generic;
using System.Linq;

namespace EverChosenServer
{
    internal static class MatchingManager
    {
        // Clients wait matching.
        public static List<Client> Clients = new List<Client>();

        private enum TeamColor
        {
            None,
            BLUE,
            RED
        };

        internal static Client MatchProcess(Client client)
        {
            if (Clients.Any())
            {
                var randomNumber = new Random();
                
                var opponent = Clients[randomNumber.Next(0, Clients.Count)];
                Clients.Remove(opponent);
                
                opponent.MatchingData.TeamColor = (int) TeamColor.BLUE;
                client.MatchingData.TeamColor = (int) TeamColor.RED;

                return opponent;
            }
            else
            {
                Clients.Add(client);
                return null;
            }
        }

        internal static bool MatchCancelProcess(Client client)
        {
            return Clients.Remove(client);
        }
    }
}