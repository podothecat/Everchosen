using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EverChosenServer.Ingame_Module;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using EverChosenPacketLib;
using MongoDB.Driver.Core.Connections;

namespace EverChosenServer
{
    internal static class DatabaseManager
    {
        private static string _connectString { get; set; }
        private static MongoClient _mongoClient { get; set; }
        private static IMongoDatabase _database { get; set; }
        private static IMongoCollection<BsonDocument> _users { get; set; }
        private static IMongoCollection<BsonDocument> _maps { get; set; }
        private static double[,] _synastry =
            {
                { 1.0, 1.2, 0.7, 0.2},
                { 0.8, 1.0, 1.5, 0.4},
                { 1.3, 0.5, 1.0, 1.7},
                { 1.8, 1.6, 0.3, 1.0}
            };

        /// <summary>
        /// Initialize information variables from database for game.
        /// </summary>
        internal static void Initialize()
        {
            _mongoClient = new MongoClient(
                ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString);
            _database = _mongoClient.GetDatabase(
                ConfigurationManager.ConnectionStrings["DatabaseName"].ConnectionString);
            _users = _database.GetCollection<BsonDocument>(
                ConfigurationManager.ConnectionStrings["DBUsersCollection"].ConnectionString);
            _maps = _database.GetCollection<BsonDocument>(
                ConfigurationManager.ConnectionStrings["DBMapsCollections"].ConnectionString);
        }

        /// <summary>
        /// When client connect to server, get client's profile from DB.
        /// </summary>
        /// <param name="clientUniqueId"></param>
        /// <returns> Client's profile </returns>
        internal static MyProfileInfo GetClientInfo(string clientUniqueId)
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
            
            var userProfile = new MyProfileInfo
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
            _users.UpdateOneAsync(filter, update);

            return nickName;
        }

        /// <summary>
        /// Get map data when matching was succeed.
        /// </summary>
        /// <returns> Map data </returns>
        internal static MapInfo GetMapInfo()
        {
            var mapName = string.Empty;

            var filter = Builders<BsonDocument>.Filter.Eq("_id", "0");
            var result = _maps.Find(filter).ToListAsync().Result;

            if (result.Count != 1)
                return null;

            var map = JsonConvert.DeserializeObject<MapInfo>(result[0].ToString());

            return map;
        }

        /// <summary>
        /// Get synastry of units.
        /// </summary>
        /// <returns> Synastry data. </returns>
        internal static double[,] GetSynastry()
        {
            return _synastry;
        }
    }
}
