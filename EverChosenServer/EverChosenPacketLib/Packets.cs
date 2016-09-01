using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EverChosenPacketLib
{
    /// <summary>
    /// Data template for communication
    /// </summary>
    public class Packet
    {
        public string MsgName { get; set; }
        public string Data { get; set; }

        public Packet(string msgName, string data)
        {
            MsgName = msgName;
            Data = data;
        }
    }

    /// <summary>
    /// Data for client information.
    /// </summary>
    public class ProfileInfo
    {
        public string NickName { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
    }

    /// <summary>
    /// Data for matching clients.
    /// </summary>
    public class MatchingInfo
    {
        public string Tribe { get; set; }
        public int Spell { get; set; }
        public int TeamColor { get; set; }

        public MatchingInfo(string tribe, int spell, int teamColor)
        {
            Tribe = tribe;
            Spell = spell;
            TeamColor = teamColor;
        }
    }

    /// <summary>
    /// Map data from server to client.
    /// </summary>
    public class MapInfo
    {
        public string MapName { get; set; }
        public List<Building> MapNodes { get; set; }
    }

    /// <summary>
    /// Building data which is used in server and client.
    /// </summary>
    public class Building
    {
        public int Owner { get; set; }
        public int Kinds { get; set; }
        public double XPos { get; set; }
        public double ZPos { get; set; }
        public int UnitCount { get; set; }
    }

    /// <summary>
    /// Unit data from server to client.
    /// </summary>
    public class Unit
    {
        public Building Units { get; set; }
        public int EndNode { get; set; }
    }

    /// <summary>
    /// To move unit data from client to server.
    /// </summary>
    public class MoveUnitInfo
    {
        public int StartNode { get; set; }
        public int EndNode { get; set; }
    }

    /// <summary>
    /// To change building data from client to server.
    /// </summary>
    public class ChangeBuildingInfo
    {
        public int Node { get; set; }
        public int Kinds { get; set; }
    }
}
