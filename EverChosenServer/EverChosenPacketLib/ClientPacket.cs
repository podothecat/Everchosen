using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EverChosenPacketLib
{
    /// <summary>
    /// To move unit data from client to server.
    /// </summary>
    public class MoveUnitInfo : Packet
    {
        public int StartNode { get; set; }
        public int EndNode { get; set; }
    }

    /// <summary>
    /// To change building data from client to server.
    /// </summary>
    public class ChangeBuildingInfo : Packet
    {
        public int Node { get; set; }
        public int Kinds { get; set; }
    }
}
