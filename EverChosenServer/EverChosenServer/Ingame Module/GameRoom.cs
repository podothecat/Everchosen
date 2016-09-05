using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MongoDB.Driver.Core.Operations;
using Newtonsoft.Json;
using EverChosenPacketLib;
using MongoDB.Driver.Core.Events;

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

        public enum TeamColor
        {
            NONE,
            BLUE,
            RED
        };
        
        private Client _player1;
        private Client _player2;
        private MapInfo _map;
        private Timer _checkConnection;
        private Timer[] _timers;
        private bool _gameProgress;

        public GameRoom(Client p1, Client p2, MapInfo map)
        {
            p1.InGameRequest += IngameCommand;
            p2.InGameRequest += IngameCommand;
            _player1 = p1;
            _player2 = p2;
            
            _map = map;
            _gameProgress = true;
            _checkConnection = new Timer(10000);
            _checkConnection.Elapsed += CheckConnection;
            _checkConnection.Start();

            _timers = new Timer[_map.MapNodes.Count];

            Console.WriteLine("Game room was constructed.");
            Console.WriteLine(_map.MapName);

            ConscriptUnit(0, 1000.0);
            ConscriptUnit(1, 1000.0);
        }

        public void CheckConnection(object s, ElapsedEventArgs e)
        {
            Console.Write("Check Connection : ");
            if (_player1.Sock.Connected && _player2.Sock.Connected)
            {
                Console.WriteLine("Matching connection is smooth.");
                _gameProgress = true;
            }
            else if (!_player1.Sock.Connected)
            {
                Console.WriteLine("Player 1 was Disconnected.");
                _player2.BeginSend("Win", null);
                _gameProgress = false;
                Release();
                IngameManager.DelRoom(this);
                
            }
            else if (!_player2.Sock.Connected)
            {
                Console.WriteLine("Player 2 was Disconnected.");
                _player1.BeginSend("Win", null);
                _gameProgress = false;
                Release();
                IngameManager.DelRoom(this);
            }
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
                    var nodes = JsonConvert.DeserializeObject<MoveUnitInfo>(e.Data);
                    var moveInfo = Move(client, nodes.StartNode, nodes.EndNode);

                    client.BeginSend(e.MsgName + "Mine", moveInfo);
                    target.BeginSend(e.MsgName + "Oppo", moveInfo);
                    break;

                case "Change":
                    var option = JsonConvert.DeserializeObject<ChangeBuildingInfo>(e.Data);
                    var buildinfo = ChangeUnit(option.Node, option.Kinds);

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

        /// <summary>
        /// Process to move units node to node.
        /// </summary>
        /// <param name="c"> Client who wants to move units. </param>
        /// <param name="startNode"> Start node of building. </param>
        /// <param name="endNode"> Destination node of building. </param>
        /// <returns> Information of units. </returns>
        private Unit Move(Client c, int startNode, int endNode)
        {
            var units = _map.MapNodes[startNode].UnitCount /= 2;
            Console.WriteLine(units);
            var unitNode = _map.MapNodes[startNode];
            unitNode.UnitCount = units;
            unitNode.Owner = c.MatchingData.TeamColor;

            var info = new Unit
            {
                Units = unitNode,
                StartNode = startNode,
                EndNode = endNode
            };
            
            return info;
        }

        /// <summary>
        /// Process to change building of player.
        /// </summary>
        /// <param name="idx"> Node index of building. </param>
        /// <param name="kinds"> Unit kinds to change. </param>
        /// <returns> Information of building. </returns>
        private Building ChangeUnit(int idx, int kinds)
        {
            _map.MapNodes[idx].Kinds = kinds;
            _map.MapNodes[idx].UnitCount = 0;

            ConscriptUnit(idx, 2000);
            return _map.MapNodes[idx];
        }

        private void Fight(Building attacker, int fightBuildingIdx)
        {
            var defender = _map.MapNodes[fightBuildingIdx];

            CheckSynastry(attacker.Kinds, defender.Kinds);
        }

        private void CheckSynastry(int kind, int kind2)
        {
            
        }

        /// <summary>
        /// Process to conscript units in building. 
        /// </summary>
        /// <param name="buildingIdx"> Building index of map for conscription. </param>
        /// <param name="conscriptionTime"> Unit spawn time frequency. </param>
        private void ConscriptUnit(int buildingIdx, double conscriptionTime)
        {
            _timers[buildingIdx] = new Timer(conscriptionTime);

            _timers[buildingIdx].Elapsed += (s, e) => OnCreateUnit(s, e, buildingIdx);
            _timers[buildingIdx].Start();
        }

        /// <summary>
        /// Timer function of conscription units.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        /// <param name="buildingIdx"> Building index of map for conscription. </param>
        private void OnCreateUnit(object source, ElapsedEventArgs e, int buildingIdx)
        {
            _map.MapNodes[buildingIdx].UnitCount += 1;
            Console.WriteLine("Building " + buildingIdx + " : " + _map.MapNodes[buildingIdx].UnitCount);
        }


        private void UseSpell()
        {
            // Need to discuss.
        }

        

        private void Result()
        {
            
        }

        private void Release()
        {
            foreach (var t in _timers)
            {
                if(t.Enabled)
                    t.Close();
            }
        }
    }
}
