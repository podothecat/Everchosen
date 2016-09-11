using System;
using System.Timers;
using Newtonsoft.Json;
using EverChosenPacketLib;

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

        public enum Outcome
        {
            DRAW,
            WIN,
            LOSE
        };
        
        private Client _player1;
        private Client _player2;
        private MapInfo _map;
        private Timer _checkConnection;
        private Timer[] _timers;
        private double[,] _synastry;

        /// <summary>
        /// Constructor of GameRoom class.
        /// </summary>
        /// <param name="p1"> Client of player 1. </param>
        /// <param name="p2"> Client of player 2. </param>
        /// <param name="map"> Map that both players will fight. </param>
        public GameRoom(Client p1, Client p2, MapInfo map)
        {
            p1.InGameRequest += IngameCommand;
            p2.InGameRequest += IngameCommand;
            _player1 = p1;
            _player2 = p2;
            
            _map = map;
            _checkConnection = new Timer(10000);
            _checkConnection.Elapsed += CheckConnection;
            _checkConnection.Start();

            _timers = new Timer[_map.MapNodes.Count];

            _synastry = DatabaseManager.GetSynastry();

            Console.WriteLine("Game room was constructed.");
            Console.WriteLine(_map.MapName);

            ConscriptUnit(0, 1000.0);
            ConscriptUnit(1, 1000.0);
        }

        /// <summary>
        /// Timer function for check disconnection.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        public void CheckConnection(object s, ElapsedEventArgs e)
        {
            Console.Write("Check Connection : ");
            if (_player1.Sock.Connected && _player2.Sock.Connected)
            {
                Console.WriteLine("Matching connection is smooth.");
            }
            else if (!_player1.Sock.Connected && !_player2.Sock.Connected)
            {
                Console.WriteLine("Both players were Disconnected.");
                Release();
            }
            else if (!_player1.Sock.Connected)
            {
                Console.WriteLine("Player 1 was Disconnected.");
                
                _player2.BeginSend(new OutcomeInfo { Outcome = (int)Outcome.WIN});
                Release();
                IngameManager.DelRoom(this);
                
            }
            else if (!_player2.Sock.Connected)
            {
                Console.WriteLine("Player 2 was Disconnected.");
                
                _player1.BeginSend(new OutcomeInfo { Outcome = (int)Outcome.WIN });
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

            if (client == null)
            {
                Console.Write("IngameCommand : Sender is null.");
                return;
            }

            // Opponent player of client who requested.
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
                    client.BeginSend(_map);
                    break;

                case "MoveUnitInfo":
                    var nodes = JsonConvert.DeserializeObject<MoveUnitInfo>(e.Data);
                    var moveInfo = Move(client, nodes.StartNode, nodes.EndNode);

                    client.BeginSend(moveInfo);
                    target.BeginSend(moveInfo);
                    break;

                case "ChangeBuildingInfo":
                    var option = JsonConvert.DeserializeObject<ChangeBuildingInfo>(e.Data);
                    var buildinfo = ChangeUnit(option.Node, option.Kinds);

                    client.BeginSend(buildinfo);
                    target.BeginSend(buildinfo);
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
        private UnitInfo Move(Client c, int startNode, int endNode)
        {
            var units = _map.MapNodes[startNode].UnitCount /= 2;
            Console.WriteLine(units);
            var unitNode = _map.MapNodes[startNode];
            unitNode.UnitCount = units;
            unitNode.Owner = c.MatchingData.TeamColor;

            var info = new UnitInfo
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

        /// <summary>
        /// Process to fight between players' units.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="fightBuildingIdx"></param>
        private void Fight(Building attacker, int fightBuildingIdx)
        {
            var defender = _map.MapNodes[fightBuildingIdx];

            while (attacker.UnitCount != 0 && defender.UnitCount != 0)
            {
                var damageOfAtkr = attacker.UnitCount*_synastry[attacker.Kinds, defender.Kinds];
                var damageOfDfnr = defender.UnitCount*_synastry[defender.Kinds, attacker.Kinds];

                defender.UnitCount -= (int)Math.Round(damageOfAtkr);
                attacker.UnitCount -= (int)Math.Round(damageOfDfnr);
            }
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
            // Need to design.
        }
        
        private void Result()
        {
            // Need to design.
        }

        /// <summary>
        /// Release timers.
        /// </summary>
        private void Release()
        {
            Console.WriteLine("Release Timer of each building.");
            foreach (var t in _timers)
            {
                if(t.Enabled)
                    t.Close();
            }
        }
    }
}
