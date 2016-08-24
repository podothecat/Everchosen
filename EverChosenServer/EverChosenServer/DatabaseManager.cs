using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal static void Initialize()
        {
            _connectString = "mongodb://localhost";
            _mongoClient = new MongoClient(_connectString);
            _database = _mongoClient.GetDatabase("EverChosen");
            _users = _database.GetCollection<BsonDocument>("users");
        }

        /// <summary>
        /// When client connect to server, get client's profile from DB.
        /// </summary>
        /// <param name="clientUniqueId"></param>
        /// <returns></returns>
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

        internal static string SetClientInfo(string nickName, string clientUniqueId)
        {
            // Set ProfileInfo to DB when unique ID is not found.
            var filter = Builders<BsonDocument>.Filter.Eq("_id", clientUniqueId);
            var update = Builders<BsonDocument>.Update.Set("nickname", nickName)
                .CurrentDate("lastModified");
            var result = _users.UpdateOneAsync(filter, update);

            return nickName;
        }

        internal static void PrintClients()
        {
            
        }
    }
}
