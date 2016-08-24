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
        private static IMongoCollection<ProfileInfo> _users { get; set; }

        internal static void Initialize()
        {
            _connectString = "mongodb://localhost";
            _mongoClient = new MongoClient(_connectString);
            _database = _mongoClient.GetDatabase("EverChosen");
            _users = _database.GetCollection<ProfileInfo>("USERS");
        }

        internal static void GetClientInfo(string clientUniqueId)
        {
            // Get ProfileInfo from DB
            //var userProfile = _users.FindAsync()
            //_users.Distinct()
        }

        internal static void SetClientInfo(Client client)
        {
            // Set ProfileInfo to DB when unique ID is not found.
        }

        internal static void PrintClients()
        {
            
        }
    }
}
