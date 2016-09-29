using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EverChosenPacketLib;
using EverChosenServer.Ingame_Module;

namespace EverChosenServer
{
    /// <summary>
    /// This class manages client list and sending packets of game.
    /// </summary>
    internal static class GameManager
    {
        private static List<Client> _clients = new List<Client>();
        
        /// <summary>
        /// Add connected client to list
        /// </summary>
        /// <param name="c"> Connected client. </param>
        public static void AddClient(Client c)
        {
            _clients.Add(c);
            Console.WriteLine("\nGAME MANAGER : client was added.");
            PrintConnectedClients();
        }

        /// <summary>
        /// Remove disconnected client from list.
        /// </summary>
        /// <param name="c"> Disconnected client. </param>
        public static void ReleaseClient(Client c)
        {
            _clients.Remove(c);
            Console.WriteLine("\nGAME MANAGER : client was removed.");
            PrintConnectedClients();
        }

        /// <summary>
        /// Find client when reconnect to server.
        /// </summary>
        /// <param name="c"> Reconnected client. </param>
        /// <param name="deviceId"> Reconnected client's device ID. </param>
        /// <returns></returns>
        public static bool FindClient(Client c, string deviceId)
        {
            var existingClient = _clients.Find(x => x.UniqueId.Equals(deviceId));

            if (existingClient == null)
                return false;

            if (existingClient.IsLogin)
            {
                c.ProfileData = existingClient.ProfileData;
                c.MatchingData = existingClient.MatchingData;
                c.IsIngame = existingClient.IsIngame;
                c.IsReadyToBattle = existingClient.IsReadyToBattle;
                c.IsReadyToFight = existingClient.IsReadyToFight;
                c.IsLogin = existingClient.IsLogin;
                if(existingClient.IsIngame)
                    Console.WriteLine(IngameManager.FindRoom(existingClient, c));
                ReleaseClient(existingClient);
                AddClient(c);
            }

            return true;
        }

        /// <summary>
        /// Get opponent client from matching queue.
        /// </summary>
        /// <param name="client"> Client who request matching. </param>
        public static void MatchingRequest(Client client)
        {
            var oppoClient = MatchingManager.MatchProcess(client);

            // There is no user waiting queue.
            if (oppoClient == null)
                return;
            
            var clientProfile = new EnemyProfileInfo
            {
                NickName = client.ProfileData.NickName,
                Wins = client.ProfileData.Wins,
                Loses = client.ProfileData.Loses
            };

            var opponentProfile = new EnemyProfileInfo
            {
                NickName = oppoClient.ProfileData.NickName,
                Wins = oppoClient.ProfileData.Wins,
                Loses = oppoClient.ProfileData.Loses
            };

            // Matching Data : Tribe, Spell, Team color
            oppoClient.BeginSend(client.MatchingData);
            client.BeginSend(oppoClient.MatchingData);

            // Profile : Nickname, Wins, Loses
            oppoClient.BeginSend(clientProfile);
            client.BeginSend(opponentProfile);
            
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

        /// <summary>
        /// Print # of connected clients to server soncole.
        /// </summary>
        public static void PrintConnectedClients()
        {
            Console.WriteLine("# of connected clients : " + _clients.Count);
        }
    }
}
