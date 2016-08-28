using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EverChosenServer.Ingame_Module;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;

namespace EverChosenServer
{
    internal static class DatabaseManager
    {
        private static string _connectString { get; set; }
        private static MongoClient _mongoClient { get; set; }
        private static IMongoDatabase _database { get; set; }
        private static IMongoCollection<BsonDocument> _users { get; set; }
        private static IMongoCollection<BsonDocument> _maps { get; set; }

        internal static void Initialize()
        {
            _connectString = "mongodb://localhost";
            _mongoClient = new MongoClient(_connectString);
            _database = _mongoClient.GetDatabase("EverChosen");
            _users = _database.GetCollection<BsonDocument>("users");
            _maps = _database.GetCollection<BsonDocument>("maps");
        }

        /// <summary>
        /// When client connect to server, get client's profile from DB.
        /// </summary>
        /// <param name="clientUniqueId"></param>
        /// <returns> Client's profile </returns>
        internal static ProfileInfo GetClientInfo(string clientUniqueId)
        {
            // Get ProfileInfo from DB           
            var filter = Builders<BsonDocument>.Filter.Eq("_id", clientUniqueId);
            var result = _users.Find(filter).ToListAsync().Result;

            // Client's profile isn't exist in DB, then make temporary profile.
            if (result.Capacity == 0)
            {
                var newProfile = new BsonDocument
                {
                    {"_id", clientUniqueId},
                    {"nickname", "닉네임을 변경해 주세요."},
                    {"wins", "0"},
                    {"loses", "0"}
                };
                _users.InsertOneAsync(newProfile);
                Console.WriteLine("New user was added.");
                result.Add(newProfile);
            }

            var userProfile = new ProfileInfo
            {
                NickName = result[0]["nickname"].AsString,
                Wins = int.Parse(result[0]["wins"].AsString),
                Loses = int.Parse(result[0]["loses"].AsString)
            };

            return userProfile;
        }

        /// <summary>
        /// Change client's nickname.
        /// </summary>
        /// <param name="nickName"> Nickname to change. </param>
        /// <param name="clientUniqueId"> Client's device id. </param>
        /// <returns> Nickname </returns>
        internal static string SetClientInfo(string nickName, string clientUniqueId)
        {
            // Set ProfileInfo to DB when unique ID is not found.
            var filter = Builders<BsonDocument>.Filter.Eq("_id", clientUniqueId);
            var update = Builders<BsonDocument>.Update.Set("nickname", nickName)
                .CurrentDate("lastModified");
            var result = _users.UpdateOneAsync(filter, update);

            return nickName;
        }

        /// <summary>
        /// Get map data when matching was succeed.
        /// </summary>
        /// <returns> Map data </returns>
        internal static GameRoom.MapInfo GetMapInfo()
        {
            var mapName = string.Empty;

            var filter = Builders<BsonDocument>.Filter.Eq("_id", "2");
            var result = _maps.Find(filter).ToListAsync().Result;

            if (result.Count != 1)
                return null;

            mapName = result[0]["mapname"].AsString;
            var nodeCount = (int)result[0]["nodes"].AsDouble;
            var positions = result[0]["positions"];

            var nodes = new List<GameRoom.Building>();
            for (int i = 0; i < nodeCount; ++i)
            {
                var x = positions[i]["x"].AsDouble;
                var z = positions[i]["z"].AsDouble;
                var o = 0;
                var k = 0;
                var b = new GameRoom.Building
                {
                    Owner = o,
                    //Kinds = k,
                    XPos = x,
                    ZPos = z,
                    //UnitCount = 0
                };
                nodes.Add(b);
            }
            
            var mapInfo = new GameRoom.MapInfo
            {
                MapName = mapName,
                MapNodes = nodes
            };

            return mapInfo;
        }

        internal static void PrintClients()
        {
            
        }
    }
}
