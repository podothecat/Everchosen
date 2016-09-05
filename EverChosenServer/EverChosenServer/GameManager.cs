using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EverChosenServer.Ingame_Module;

namespace EverChosenServer
{
    /// <summary>
    /// This class manages client list and sending packets of game.
    /// </summary>
    internal static class GameManager
    {
        public static List<Client> Clients = new List<Client>();
        
        /// <summary>
        /// Add connected client to list
        /// </summary>
        /// <param name="c"> Connected client. </param>
        public static void AddClient(Client c)
        {
            Clients.Add(c);
            Console.WriteLine("\nGAME MANAGER : client was added.");
            PrintConnectedClients();
        }

        /// <summary>
        /// Remove disconnected client from list.
        /// </summary>
        /// <param name="c"> Disconnected client. </param>
        public static void ReleaseClient(Client c)
        {
            Clients.Remove(c);
            Console.WriteLine("\nGAME MANAGER : client was removed.");
            PrintConnectedClients();
        }

        /// <summary>
        /// Print # of connected clients to server soncole.
        /// </summary>
        public static void PrintConnectedClients()
        {
            Console.WriteLine("# of connected clients : " + Clients.Count);
        }

        /// <summary>
        /// Get opponent client from matching queue.
        /// </summary>
        /// <param name="client"> Client who request matching. </param>
        public static void MatchingRequest(Client client)
        {
            var oppoClient = MatchingManager.MatchProcess(client);

            if (oppoClient == null)
                return;
            
            oppoClient.BeginSend("OnSucceedMatching1", client.MatchingData);
            client.BeginSend("OnSucceedMatching1", oppoClient.MatchingData);
            oppoClient.BeginSend("OnSucceedMatching2", client.LoginData);
            client.BeginSend("OnSucceedMatching2", oppoClient.LoginData);
            oppoClient.IsIngame = true;
            client.IsIngame = true;

            var map = DatabaseManager.GetMapInfo();
            IngameManager.AddRoom(new GameRoom(oppoClient, client, map));
        }

        /// <summary>
        /// Remove client that requested matching.
        /// </summary>
        /// <param name="client"> Client who request to cancel matching. </param>
        public static void MatchingCancelRequest(Client client)
        {
            if(!MatchingManager.MatchCancelProcess(client))
                Console.WriteLine("Matching Cancel Error.");
        }
    }
}
