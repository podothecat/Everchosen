using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EverChosenServer
{
    /// <summary>
    /// Data template for communication
    /// </summary>
    public class Packet
    {
        public string MsgName { get; set; }
        public string Data { get; set; }

        public Packet(string msgName, object data)
        {
            MsgName = msgName;
            Data = JsonConvert.SerializeObject(data);
        }
    }

    /// <summary>
    /// Data for client information.
    /// </summary>
    public class LoginPacket
    {
        public string NickName { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
    }

    /// <summary>
    /// Data for matching clients.
    /// </summary>
    public class MatchingPacket
    {
        public string Id { get; set; }
        public string Tribe { get; set; }
        public int Spell { get; set; }
        public int TeamColor { get; set; }
    }

    /// <summary>
    /// Data for processing game logic of two clients
    /// </summary>
    public class IngamePacket
    {
        public int UnitCount { get; set; }
    }
}
