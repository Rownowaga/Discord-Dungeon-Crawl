using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMaster.Database
{
    internal class MongoDBConnection
    {
        private readonly IMongoDatabase database;

        public MongoDBConnection()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            database = client.GetDatabase("DiscordDungeon");    
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task upsertDocument(string collectionName, string key, string json)
        {
            var collection = GetCollection(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("name", key);
            var document = collection.Find(filter).FirstOrDefault();

            if (document != null)
            {
                try
                {
                    var update = new BsonDocument("$set", BsonDocument.Parse(json));
                    await collection.UpdateOneAsync(filter, update);
                }
                catch 
                { 
                    Console.WriteLine("failed to update " + key);
                }
            }
            else
            {
                await collection.InsertOneAsync(BsonDocument.Parse(json));
                Console.WriteLine("New record created: " + key);
            }

        }

        public BsonDocument selectDocument(string collectionName, string key, string value)
        {
            var collection = GetCollection(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq(key, value);
            var document = collection.Find(filter).FirstOrDefault();
            return document;
        }
    }
}
