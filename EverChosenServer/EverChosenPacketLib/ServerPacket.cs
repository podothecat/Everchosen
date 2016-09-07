﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace EverChosenPacketLib
{
    
    /// <summary>
    /// Map data from server to client.
    /// </summary>
    public class MapInfo : Packet
    {
        public string MapName { get; set; }
        public List<Building> MapNodes { get; set; }
    }

    /// <summary>
    /// Unit data from server to client.
    /// </summary>
    public class Unit : Packet
    {
        public Building Units { get; set; }
        public int StartNode { get; set; }
        public int EndNode { get; set; }
    }

    /// <summary>
    /// Result data of matching from server to client.
    /// </summary>
    public class OutcomeInfo : Packet
    {
        public int Outcome { get; set; }
    }
}
