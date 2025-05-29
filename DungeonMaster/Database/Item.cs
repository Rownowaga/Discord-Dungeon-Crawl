using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DungeonMaster.Database
{
    internal class Item
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }

        [BsonElement("name")]
        public string name { get; set; }

        [BsonElement("desc")]
        public string desc { get; set; }

        [BsonElement("quantity")]
        public int quantity { get; set; }

        [BsonElement("rarity")]
        public ItemRarity rarity { get; set; }

        [BsonElement("consumeDesc")]
        public string consumeDesc { get; set; }

        [BsonElement("damage")]
        public int damage { get; set; }

        [BsonElement("armor")]

        public int armor { get; set; }


        [BsonElement("isLightSource")]
        public bool isLightSource = false;

        [BsonElement("isWeapon")]
        public bool isWeapon = false;


        [BsonElement("isSpellbook")]
        public bool isSpellbook = false;


        [BsonElement("isArmor")]
        public bool isArmor = false;


        [BsonElement("isConsumable")]
        public bool isConsumable = false;


        [BsonElement("isLeatherTool")]
        public bool isLeatherTool = false;


        [BsonElement("isPotionTool")]
        public bool isPotionTool = false;


        [BsonElement("isCookingTool")]
        public bool isCookingTool = false;


        [BsonElement("isBlacksmithingTool")]
        public bool isBlacksmithingTool = false;
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

}
