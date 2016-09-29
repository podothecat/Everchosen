using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace EverChosenServer.Ingame_Module
{
    internal static class IngameManager
    {
        private static List<GameRoom> _rooms = new List<GameRoom>();

        /// <summary>
        /// Add game when matching was started.
        /// </summary>
        /// <param name="room"></param>
        public static void AddRoom(GameRoom room)
        {
            Console.WriteLine("Ingame Manager : Add game room.");
            _rooms.Add(room);
        }

        /// <summary>
        /// Delete game when matching was done or disconnection was occured.
        /// </summary>
        /// <param name="room"></param>
        public static void DelRoom(GameRoom room)
        {
            Console.WriteLine("Ingame Manager : Del game room.");
            _rooms.Remove(room);
        }

        public static bool FindRoom(Client c1, Client c2)
        {
            var result = _rooms.Find(x => x.FindPlayer(c1, c2));

            return result != null;
        }
    }
}
