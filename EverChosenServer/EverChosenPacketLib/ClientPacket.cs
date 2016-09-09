﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

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

    /// <summary>
    /// To fight between units.
    /// </summary>
    public class FightInfo : Packet
    {
        public Building Units { get; set; }
        public int FightBuildingIdx { get; set; }
    }

    /// <summary>
    /// To use spell to building or units.
    /// </summary>
    public class SpellInfo : Packet
    {
        public int Spell { get; set; }
        public int SpellBuildingIdx { get; set; }
    }

    public class LoginInfo : Packet
    {
        public string DeviceId { get; set; }
    }

    public class MapReq : Packet
    {
        public string Req { get; set; }
    }

    public class ExitReq : Packet
    {
        public string Req { get; set; }
    }

    public class QueueCancelReq : Packet
    {
        public string Req { get; set; }
    }
}
