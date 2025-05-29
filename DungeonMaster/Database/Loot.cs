using System;
using System.Collections.Generic;

namespace DungeonMaster.Database
{
    internal class Loot
    {
        public string name { get; set; }

        public List<string> roomList { get; set; } = new List<string>();

        public double dropRate { get; set; }

    }
}
