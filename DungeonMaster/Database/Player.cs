using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMaster.Database
{
    internal class Player
    {
        public ObjectId _id { get; set; }
        public string name { get; set; }
        public int currentHealth { get; set; }
        public int currentMana { get; set; }
        public int maxHealth { get; set; }
        public int maxMana { get; set; }
        public Item[] inventory { get; set; }

        public int roomRecord = 1;

        public int starterAllowance = 3;

        public bool hasItem(string itemName)
        {
            foreach (Item item in inventory)
            {
                if(item != null && item.name == itemName)
                    return true;
            }
            return false;
        }

        //Returns -1 if there is no space, else returns the free slot
        public int hasFreeSpace()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == null)
                    return i;
            }
            return -1;
        }

    }
}
