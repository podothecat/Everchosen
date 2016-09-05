using System;
using System.Collections.Generic;
using System.Linq;
using EverChosenServer.Ingame_Module;

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
                var randomNumber = new Random();
                
                var opponent = Clients[randomNumber.Next(0, Clients.Count)];
                Clients.Remove(opponent);
                
                opponent.MatchingData.TeamColor = (int) GameRoom.TeamColor.BLUE;
                client.MatchingData.TeamColor = (int) GameRoom.TeamColor.RED;

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