using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine("\nGAME MANAGER : client was added.\n");
            PrintConnectedClients();
        }

        /// <summary>
        /// Remove disconnected client from list.
        /// </summary>
        /// <param name="c"> Disconnected client. </param>
        public static void ReleaseClient(Client c)
        {
            Clients.Remove(c);

            Console.WriteLine("\nGAME MANAGER : client was removed.\n");
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
        /// Get client info from DB whis has same device ID.
        /// </summary>
        /// <param name="client"> Accepted client. </param>
        public static void LoginRequest(Client client)
        {
            //DatabaseManager.GetClientInfo(client);

            // Write code to get Login Information from DB (now temporary)
            var nick = "Ragdoll";
            var wins = 10;
            var loses = 5;
            // ...
            
            client.LoginData = new ProfilePacket
            {
                NickName = nick,
                Wins = wins,
                Loses = loses
            };
            client.BeginSend("OnSucceedLogin", client.LoginData);
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

            oppoClient.BeginSend("OnSucceedMatching", client.MatchingData);
            oppoClient.IsIngame = true;
            client.BeginSend("OnSucceedMatching", oppoClient.MatchingData);
            client.IsIngame = true;
            var ingame = new IngameManager(client, oppoClient);
        }

        /// <summary>
        /// Remove client that requested matching.
        /// </summary>
        /// <param name="client"> Client who request to cancel matching. </param>
        public static void MatchingCancelRequest(Client client)
        {
            MatchingManager.MatchCancelProcess(client);
        }

        /// <summary>
        /// Process ingame request and send result to clients.
        /// </summary>
        /// <param name="client"></param>
        public static void IngameRequest(Client client)
        {
            // Write code for game logic.
        }
    }
}
