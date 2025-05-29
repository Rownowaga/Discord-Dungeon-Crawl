using Discord;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMaster.Database
{
    internal class Path
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string name { get; set; }

        [BsonElement("northRoom")]
        public string northRoom { get; set; }

        [BsonElement("southRoom")]
        public string southRoom { get; set; }

        [BsonElement("westRoom")]
        public string westRoom { get; set; }

        [BsonElement("eastRoom")]
        public string eastRoom { get; set; }

        [BsonElement("veerWest")]
        public string veerWest { get; set; }

        [BsonElement("veerEast")]
        public string veerEast { get; set; }
    }
}
