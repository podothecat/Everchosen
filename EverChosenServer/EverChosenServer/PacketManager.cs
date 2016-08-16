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

    /*  This code may be reused..
        public static class SplitParameter
        {
            public static char[] delimiterChars =
            {
                '{', '}', '"', ':', ',', '\n', ' ', '\t', '\r'
            };
        }

        internal interface IPacket
        {
            string MsgName { get; set; }
            dynamic Data { get; set; }
        }

        public class PacketConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPacket));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jo = JObject.Load(reader);

                if (jo["MsgName"].Value<string>() == "OnMatchingRequest")
                {
                    return jo.ToObject<MatchingPacket>(serializer);
                }
                if (jo["MsgName"].Value<string>() == "OnInGameRequest")
                {
                    return jo.ToObject<InGamePacket>(serializer);
                }

                return null;
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Packet which will be used in Matching Scene.
        /// </summary>
        internal class MatchingPacket : IPacket
        {
            public string MsgName { get; set; }
            public dynamic Data { get; set; }

            public string Id { get; set; }
            public string Tribe { get; set; }
            public int Spell { get; set; }

            public MatchingPacket(string msgName, dynamic data)
            {
                MsgName = msgName;
                Data = data;

                ParseData();
            }

            private void ParseData()
            {
                string d = Data.ToString();

                var tags = d.Split(SplitParameter.delimiterChars, StringSplitOptions.RemoveEmptyEntries);

                Id = tags[1];
                Tribe = tags[3];
                Spell = Convert.ToInt32(tags[5]);

                Console.WriteLine(Id + " " + Tribe + " " + Spell);
            }
        }

        /// <summary>
        /// Packet which will be used in InGame Scene.
        /// </summary>
        internal class InGamePacket : IPacket
        {
            public string MsgName { get; set; }
            public dynamic Data { get; set; }
        }
    */
}
