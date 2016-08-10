using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverChosenServer
{
    internal static class GameManager
    {
        public static List<Client> _clients = new List<Client>();

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

                opponent.SendPacket();
                client.SendPacket();
            }
            else
            {
                Clients.Enqueue(client);
            }
        }
    }

    public class MatchingRequestParam
    {
        public int SkillId { get; set; }
        public int Race { get; set; }
        public int ProfileIcon { get; set; }
    }
}
