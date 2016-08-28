using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Operations;
using Newtonsoft.Json;

namespace EverChosenServer.Ingame_Module
{
    internal class GameRoom
    {
        /* ---------------------------
         * Manage two players
         * 
         * Activity of players
         * 1. Move unit. (start building, destination building and # of units.)
         * 2. Change building option.
         * 3. Occupy building of enemy of normal.
         * 4. Fight against the enemy.
         * 5. Increment unit count in building automatically.
         * 6. Surrender game.
         * 7. Win or lose game.
        ----------------------------*/

        private Client _player1;
        private Client _player2;
        private MapInfo _map;

        /// <summary>
        /// Data for move units from building to building.
        /// </summary>
        private class MoveInfo
        {
            public int StartNode { get; set; }
            public int EndNode { get; set; }
            public int UnitCount { get; set; }
        }

        /// <summary>
        /// Data of one building.
        /// </summary>
        public class Building
        {
            public int Owner { get; set; }
            //public int Kinds { get; set; }
            public double XPos { get; set; }
            public double ZPos { get; set; }
            //public int UnitCount { get; set; }
        }

        public class MapInfo
        {
            public string MapName { get; set; }
            public List<Building> MapNodes { get; set; }
        }

        private class BuildingInfo
        {
            public int Node { get; set; }
            public int Kinds { get; set; }
        }

        public GameRoom(Client a1, Client a2, MapInfo map)
        {
            _player1 = a1;
            _player2 = a2;
            _map = map;
            _map.MapNodes[0].Owner = 1;
            _map.MapNodes[_map.MapNodes.Count-1].Owner = 2;
            Console.WriteLine("Game room was constructed.");
            Console.WriteLine(_map.MapName);
        }

        /// <summary>
        /// Attached to EventHandler IngameRequest()
        /// </summary>
        /// <param name="sender"> Client who requested. </param>
        /// <param name="e"> Data </param>
        public void IngameCommand(object sender, Packet e)
        {
            var client = sender as Client;
            Client target;
            
            if (client == _player1)
                target = _player2;
            else if (client == _player2)
                target = _player1;
            else
            {
                Console.WriteLine("Invalid object.");
                return;
            }

            switch (e.MsgName)
            {
                case "MapReq":
                    client.BeginSend("MapInfo", _map);
                    break;

                case "Move":
                    var nodes = JsonConvert.DeserializeObject<MoveInfo>(e.Data);
                    var moveInfo = Move(target, nodes.StartNode, nodes.EndNode);

                    client.BeginSend(e.MsgName + "Mine", moveInfo);
                    target.BeginSend(e.MsgName + "Oppo", moveInfo);
                    break;

                case "Change":
                    var option = JsonConvert.DeserializeObject<BuildingInfo>(e.Data);
                    var buildinfo = ChangeUnit(target, option.Node, option.Kinds);

                    client.BeginSend(e.MsgName + "Mine", buildinfo);
                    target.BeginSend(e.MsgName + "Oppo", buildinfo);
                    break;

                case "Fight":
                    break;

                default:
                    Console.WriteLine(e.MsgName + " is not ingame command.");
                    break;
            }
        }

        private MoveInfo Move(Client t, int s, int e)
        {
            var movingUnit = 0;

            var info = new MoveInfo
            {
                StartNode = s,
                EndNode = e,
                UnitCount = movingUnit
            };

            return info;
        }

        private BuildingInfo ChangeUnit(Client t, int idx, int kinds)
        {
            //_mapNodes[idx].Kinds = kinds;
            var info = new BuildingInfo
            {
                Node = idx,
                Kinds = kinds
            };
            
            return info;
        }
        
        private void UseSpell()
        {
            // Need to discuss.
        }

        private void Fight()
        {
            // Need to discuss.
        }

        private void Result()
        {
            
        }
    }
}
