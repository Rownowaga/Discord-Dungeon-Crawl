using Discord;
using DungeonMaster.Database;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMaster
{
    internal class Helpers
    {
        public static string CapitalizeFirstLetter(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static Color getRarityColor(Item item)
        {
            Color returnColor = new Color();
            switch (item.rarity)
            {
                case ItemRarity.Common:
                    returnColor = Color.LighterGrey;
                    break;
                case ItemRarity.Uncommon:
                    returnColor = Color.Green;
                    break;
                case ItemRarity.Rare:
                    returnColor = Color.Blue;
                    break;
                case ItemRarity.Epic:
                    returnColor = Color.Purple;
                    break;
                case ItemRarity.Legendary:
                    returnColor = Color.Orange;
                    break;
                default:
                    Console.WriteLine("Unknown rarity.");
                    break;
            }
            return returnColor;
        }

        public static string additemProperties(Item item)
        {
            StringBuilder sb = new StringBuilder();

            if (item.isWeapon)
                sb.AppendLine($"- Acts as a **Weapon** ({item.damage} damage)");

            if (item.isArmor)
                sb.AppendLine($"- Acts as **Armor** ({item.armor} armor)");

            if (item.isSpellbook)
                sb.AppendLine($"- Acts as a **Spellbook**");

            if (item.isLightSource)
                sb.AppendLine($"- Acts as a **Light Source**");

            if (item.isBlacksmithingTool)
                sb.AppendLine($"- Acts as a **Blacksmith** tool");

            if (item.isLeatherTool)
                sb.AppendLine($"- Acts as a **Leathercraft** tool");

            if (item.isCookingTool)
                sb.AppendLine($"- Acts as a **Cooking** tool");

            if (item.isConsumable)
                sb.AppendLine($"- **Effect**: {item.consumeDesc}");


            return sb.ToString();
        }

    }
    internal class Button
    {
        [BsonElement("label")]
        public string label { get; set; }

        [BsonElement("customId")]
        public string customId { get; set; }

        [BsonElement("style")]
        public ButtonStyle style { get; set; }
        public Button(string label, string customId, ButtonStyle style)
        {
            this.label = label;
            this.customId = customId;
            this.style = style;
        }

        public static ComponentBuilder CreateButton(string label, string customId, ButtonStyle style)
        {
            return new ComponentBuilder().WithButton(label, customId, style);
        }
    }
}
