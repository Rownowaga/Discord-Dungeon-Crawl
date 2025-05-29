using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DungeonMaster.Database
{
    internal class Room
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string name { get; set; }

        [BsonElement("desc")]
        public string desc { get; set; }

        [BsonElement("title")]
        public string title { get; set; }

        [BsonElement("objective")]
        public string objective { get; set; }

        [BsonElement("tip")]
        public string tip { get; set; }

        [BsonElement("buttons")]
        public List<Button> buttons { get; set; } = new List<Button>();

    }
}
