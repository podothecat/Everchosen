using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Newtonsoft.Json;

namespace EverChosenPacketLib
{
    /// <summary>
    /// Data template for communication
    /// </summary>
    public class Packet
    {
        [JsonIgnore]
        public string MsgName => GetType().Name;
        [JsonIgnore]
        public string Data => JsonConvert.SerializeObject(this);
    }

    /// <summary>
    /// Profile data of self.
    /// </summary>
    public class MyProfileInfo : Packet
    {
        public string NickName { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
    }

    /// <summary>
    /// Profile data of enemy.
    /// </summary>
    public class EnemyProfileInfo : Packet
    {
        public string NickName { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
    }

    /// <summary>
    /// Data for matching clients.
    /// </summary>
    public class MyMatchingInfo : Packet
    {
        public string Tribe { get; set; }
        public int Spell { get; set; }
        public int TeamColor { get; set; }

        public MyMatchingInfo(string tribe, int spell, int teamColor)
        {
            Tribe = tribe;
            Spell = spell;
            TeamColor = teamColor;
        }
    }

    /// <summary>
    /// Data for matching clients.
    /// </summary>
    public class EnemyMatchingInfo : Packet
    {
        public string Tribe { get; set; }
        public int Spell { get; set; }
        public int TeamColor { get; set; }

        public EnemyMatchingInfo(string tribe, int spell, int teamColor)
        {
            Tribe = tribe;
            Spell = spell;
            TeamColor = teamColor;
        }
    }


    /// <summary>
    /// Nickname of client.
    /// </summary>
    public class NickNameInfo : Packet
    {
        public string NickName { get; set; }
    }

    /// <summary>
    /// Building data which is used in server and client.
    /// </summary>
    public class Building : Packet
    {
        public int Owner { get; set; }
        public int Kinds { get; set; }
        public double XPos { get; set; }
        public double ZPos { get; set; }
        public int UnitCount { get; set; }
    }
}