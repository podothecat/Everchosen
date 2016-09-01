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

        public static void AddRoom(GameRoom room)
        {
            _rooms.Add(room);
        }

        public static void DelRoom(GameRoom room)
        {
            _rooms.Remove(room);
        }
    }

    
}
