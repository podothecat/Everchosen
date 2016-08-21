using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EverChosenServer
{
    internal class GameRoom
    {
        /* ---------------------------
         * Manage two players
         * 
         * Activity of players
         * 1. Move unit.
         * 2. Change building option.
         * 3. Occupy building of enemy of normal.
         * 4. Fight against the enemy.
         * 5. Increment unit count in building automatically.
         * 6. Surrender game.
         * 7. Win or lose game.
        ----------------------------*/

        private Client _player1;
        private Client _player2;

        public GameRoom(Client a1, Client a2)
        {
            _player1 = a1;
            _player2 = a2;
        }

        class Move
        {
            public int StartNode { get; set; }
            public int EndNode { get; set; }
        }

        class Option
        {
            public int BuildingNode { get; set; }
            public int BuildingLevel { get; set; }
        }

        public void IngameCommand(object sender, Packet e)
        {
            var client = sender as Client;

            switch (e.MsgName)
            {
                case "Move":
                    var nodes = JsonConvert.DeserializeObject<Move>(e.Data);
                    Console.WriteLine("Move Node " + nodes.StartNode + " to " + nodes.EndNode);
                    break;

                case "Option":
                    var option = JsonConvert.DeserializeObject<Option>(e.Data);
                    Console.WriteLine("Change kind " + option.BuildingNode + " to " + option.BuildingLevel);
                    break;

                case "Fight":
                    break;

                default:
                    Console.WriteLine(e.MsgName + " is not ingame command.");
                    break;
            }

            if (client == _player1)
            {
               
            }
            else if (client == _player2)
            {

            }
            else
            {
                Console.WriteLine("Exception");
            }
        }

        private void MoveUnit(int start, int end)
        {
            
        }

        private void ChangeUnit(int node, int kind)
        {
            
        }

        private void Fight(int node, int unit1, int unit2)
        {
            
        }

        private void IncrementUnit()
        {
            
        }

        private void Surrender()
        {
            
        }

        private void Result()
        {
            
        }
    }
}
