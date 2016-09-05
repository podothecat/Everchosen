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

        /// <summary>
        /// Process matching request of client.
        /// </summary>
        /// <param name="client"> Client who request matching. </param>
        /// <returns> Returns opponent client. </returns>
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
            Clients.Add(client);
            return null;
        }

        /// <summary>
        /// Process matching cancel request of client.
        /// </summary>
        /// <param name="client"> Client who request to cancel matching. </param>
        /// <returns> Returns whether cancel is success. </returns>
        internal static bool MatchCancelProcess(Client client)
        {
            return Clients.Remove(client);
        }
    }
}