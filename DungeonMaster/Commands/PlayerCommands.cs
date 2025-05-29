using Discord.WebSocket;
using Discord;
using DungeonMaster.Database;
using MongoDB.Bson.Serialization;
using System.Text;

namespace DungeonMaster.Commands
{
    internal class PlayerCommands
    {
        private readonly MongoDBConnection _mongoDBConnection;

        public PlayerCommands()
        {
            _mongoDBConnection = new MongoDBConnection();
        }

        public async Task HandleInfoCommand(SocketSlashCommand command)
        {
            var embed = new EmbedBuilder();

            string itemName = (string)command.Data.Options.First().Value;
            Item item = BsonSerializer.Deserialize<Item>(_mongoDBConnection.selectDocument("items", "name", itemName.ToLower()));

            embed.Title = Helpers.CapitalizeFirstLetter(item.name);
            embed.Description = item.desc + "\n\n";
            embed.Description += Helpers.additemProperties(item);
            embed.Color = Helpers.getRarityColor(item);

            await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        public async Task HandleStatusCommand(SocketSlashCommand command)
        {
            var embed = new EmbedBuilder();

            Player player = BsonSerializer.Deserialize<Player>(_mongoDBConnection.selectDocument("players", "name", command.User.Username));

            embed.Title = Helpers.CapitalizeFirstLetter(player.name);
            embed.AddField("Health", $"{player.currentHealth}/{player.maxHealth}");
            embed.AddField("Mana", $"{player.currentMana}/{player.maxMana}");

            StringBuilder inventoryList = new StringBuilder();
            foreach (Item pItem in player.inventory)
            {
                if (pItem != null)
                    inventoryList.AppendLine($"- {Helpers.CapitalizeFirstLetter(pItem.name)} {pItem.quantity}x");
            }

            embed.AddField("Inventory", inventoryList.ToString() == "" ? "Empty!" : inventoryList.ToString());

            int hpStatus = player.maxHealth / player.currentHealth;
            embed.Color = Color.Green;
            if (hpStatus >= 2) //50%
                embed.Color = Color.LightOrange;
            if (hpStatus >= 4) //25%
                embed.Color = Color.Red;

            embed.AddField("Room Record", player.roomRecord);
            embed.ImageUrl = command.User.GetAvatarUrl();

            await command.RespondAsync(embed: embed.Build());
        }

        public async Task HandleInvestigateCommand(SocketSlashCommand command)
        {
            // Define probabilities for outcomes
            var outcomeOdds = new Dictionary<string, double>
            {
                { "Loot", 0.50 },
                { "Event", 0.20 },
                { "Combat", 0.30 }
            };

            // Perform a weighted roll
            var random = new Random();
            double roll = random.NextDouble();
            double cumulative = 0.0;
            string outcome = null;

            foreach (var entry in outcomeOdds)
            {
                cumulative += entry.Value;
                if (roll <= cumulative)
                {
                    outcome = entry.Key;
                    break;
                }
            }

            // Handle the outcome
            switch (outcome)
            {
                case "Loot":
                    await HandleLoot(command);
                    break;

                case "Event":
                    await HandleEvent(command);
                    break;

                case "Combat":
                    await HandleCombat(command);
                    break;

                default:
                    await command.RespondAsync("You found nothing this time. Better luck next time!", ephemeral: true);
                    break;
            }
        }

        // Handle loot outcome
        private async Task HandleLoot(SocketSlashCommand command)
        {
            var lootOdds = new Dictionary<string, double>
            {
                { "Common", 0.50 },
                { "Uncommon", 0.30 },
                { "Rare", 0.15 },
                { "Epic", 0.04 },
                { "Legendary", 0.01 }
            };

            // Perform a weighted roll for loot rarity
            var random = new Random();
            double roll = random.NextDouble();
            double cumulative = 0.0;
            string rarity = null;

            foreach (var entry in lootOdds)
            {
                cumulative += entry.Value;
                if (roll <= cumulative)
                {
                    rarity = entry.Key;
                    break;
                }
            }

            // Retrieve a random item of the selected rarity from the database
            var lootDoc = _mongoDBConnection.selectDocument("item", "rarity", rarity);
            if (lootDoc != null)
            {
                var item = BsonSerializer.Deserialize<Item>(lootDoc);
                await command.RespondAsync($"You found a **{rarity}** item: **{item.name}**!", ephemeral: true);
            }
            else
            {
                await command.RespondAsync("You found nothing this time. Better luck next time!", ephemeral: true);
            }
        }

        private async Task HandleEvent(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }

        private async Task HandleCombat(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
