using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

using DungeonMaster.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Diagnostics.Tracing;


namespace DungeonMaster
{
    internal class ButtonHandler
    {
        public static async Task ButtonActions(SocketMessageComponent component)
        {

            SocketGuildUser user = (SocketGuildUser)component.User;
            var mongoDBConnection = new MongoDBConnection();

            BsonDocument playerDoc = mongoDBConnection.selectDocument("players", "name", user.Username);
            Player player = null;
            if (playerDoc != null)
                player = BsonSerializer.Deserialize<Player>(playerDoc);


            StringBuilder response = new StringBuilder();

            switch (component.Data.CustomId)
            {
                #region Dungeon Start
                case "btnStart": //TODO: If a player leaves the server, they cannot restart the dungeon

                    if (player == null)
                    {
                        Player newPlayer = new Player();
                        newPlayer._id = ObjectId.GenerateNewId();
                        newPlayer.name = user.Username;
                        newPlayer.currentHealth = 10;
                        newPlayer.currentMana = 10;
                        newPlayer.maxHealth = 10;
                        newPlayer.maxMana = 10;
                        newPlayer.inventory = new Item[25];

                        await mongoDBConnection.upsertDocument("players", newPlayer.name, newPlayer.ToJson());

                        await user.AddRoleAsync(user.Guild.Roles.FirstOrDefault(r => r.Name == "starter"));
                        await user.AddRoleAsync(user.Guild.Roles.FirstOrDefault(r => r.Name == "tavern"));


                        await component.DeferAsync();
                    }
                    else
                        await component.DeferAsync();
                    break;

                case "btnTorch":

                    Item torch = BsonSerializer.Deserialize<Item>(mongoDBConnection.selectDocument("items", "name", "torch"));
                    player = BsonSerializer.Deserialize<Player>(mongoDBConnection.selectDocument("players", "name", user.Username));

                    torch.quantity = 1;

                    if (player.starterAllowance != 0)
                    {
                        if (player.hasItem(torch.name))
                            await component.RespondAsync($"You already have a {Helpers.CapitalizeFirstLetter(torch.name)}", ephemeral: true);
                        else
                        {
                            int slot = player.hasFreeSpace();
                            player.inventory[slot] = torch;
                            player.starterAllowance -= 1;
                            await mongoDBConnection.upsertDocument("players", player.name, player.ToJson());

                            response.AppendLine($"You collected a **{Helpers.CapitalizeFirstLetter(torch.name)}!**");

                            if (player.starterAllowance != 0)
                                response.AppendLine($"You can choose {player.starterAllowance} more item(s)...");
                            else
                                response.AppendLine($"You've gathered everything you're allowed! Goodluck on your adventure!");

                            await component.RespondAsync(response.ToString(), ephemeral: true);
                        }
                    }

                    if (player.starterAllowance == 0)
                        await enterDungeon(user);
                    break;

                case "btnSword":

                    Item sword = BsonSerializer.Deserialize<Item>(mongoDBConnection.selectDocument("items", "name", "short sword"));

                    sword.quantity = 1;

                    if (player.starterAllowance != 0)
                    {
                        if (player.hasItem(sword.name))
                            await component.RespondAsync($"You already have a {Helpers.CapitalizeFirstLetter(sword.name)}", ephemeral: true);
                        else
                        {
                            int slot = player.hasFreeSpace();
                            player.inventory[slot] = sword;
                            player.starterAllowance -= 1;
                            await mongoDBConnection.upsertDocument("players", player.name, player.ToJson());

                            response.AppendLine($"You collected a **{Helpers.CapitalizeFirstLetter(sword.name)}!**");

                            if (player.starterAllowance != 0)
                                response.AppendLine($"You can choose {player.starterAllowance} more item(s)...");
                            else
                                response.AppendLine($"You've gathered everything you're allowed! Goodluck on your adventure!");

                            await component.RespondAsync(response.ToString(), ephemeral: true);
                        }
                    }

                    if (player.starterAllowance == 0)
                        await enterDungeon(user);
                    break;

                case "btnCookingPot":

                    Item cookingPot = BsonSerializer.Deserialize<Item>(mongoDBConnection.selectDocument("items", "name", "cooking pot"));

                    cookingPot.quantity = 1;

                    if (player.starterAllowance != 0)
                    {
                        if (player.hasItem(cookingPot.name))
                            await component.RespondAsync($"You already have a {Helpers.CapitalizeFirstLetter(cookingPot.name)}", ephemeral: true);
                        else
                        {
                            int slot = player.hasFreeSpace();
                            player.inventory[slot] = cookingPot;
                            player.starterAllowance -= 1;
                            await mongoDBConnection.upsertDocument("players", player.name, player.ToJson());

                            response.AppendLine($"You collected a **{Helpers.CapitalizeFirstLetter(cookingPot.name)}!**");

                            if (player.starterAllowance != 0)
                                response.AppendLine($"You can choose {player.starterAllowance} more item(s)...");
                            else
                                response.AppendLine($"You've gathered everything you're allowed! Goodluck on your adventure!");

                            await component.RespondAsync(response.ToString(), ephemeral: true);
                        }
                    }

                    if (player.starterAllowance == 0)
                        await enterDungeon(user);
                    break;

                case "btnDagger":

                    Item dagger = BsonSerializer.Deserialize<Item>(mongoDBConnection.selectDocument("items", "name", "dagger"));

                    dagger.quantity = 1;

                    if (player.starterAllowance != 0)
                    {
                        if (player.hasItem(dagger.name))
                            await component.RespondAsync($"You already have a {Helpers.CapitalizeFirstLetter(dagger.name)}", ephemeral: true);
                        else
                        {
                            int slot = player.hasFreeSpace();
                            player.inventory[slot] = dagger;
                            player.starterAllowance -= 1;
                            await mongoDBConnection.upsertDocument("players", player.name, player.ToJson());

                            response.AppendLine($"You collected a **{Helpers.CapitalizeFirstLetter(dagger.name)}!**");

                            if (player.starterAllowance != 0)
                                response.AppendLine($"You can choose {player.starterAllowance} more item(s)...");
                            else
                                response.AppendLine($"You've gathered everything you're allowed! Goodluck on your adventure!");

                            await component.RespondAsync(response.ToString(), ephemeral: true);
                        }
                    }

                    if (player.starterAllowance == 0)
                        await enterDungeon(user);
                    break;

                case "btnFeather":

                    Item feather = BsonSerializer.Deserialize<Item>(mongoDBConnection.selectDocument("items", "name", "phoenix feather"));

                    feather.quantity = 1;

                    if (player.starterAllowance != 0)
                    {
                        if (player.hasItem(feather.name))
                            await component.RespondAsync($"You already have a {Helpers.CapitalizeFirstLetter(feather.name)}", ephemeral: true);
                        else
                        {
                            int slot = player.hasFreeSpace();
                            player.inventory[slot] = feather;
                            player.starterAllowance -= 1;
                            await mongoDBConnection.upsertDocument("players", player.name, player.ToJson());

                            response.AppendLine($"You collected a **{Helpers.CapitalizeFirstLetter(feather.name)}!**");

                            if (player.starterAllowance != 0)
                                response.AppendLine($"You can choose {player.starterAllowance} more item(s)...");
                            else
                                response.AppendLine($"You've gathered everything you're allowed! Goodluck on your adventure!");

                            await component.RespondAsync(response.ToString(), ephemeral: true);
                        }
                    }

                    if (player.starterAllowance == 0)
                        await enterDungeon(user);
                    break;
                #endregion

                #region Directions
                case "btnLeft":
                case "btnForward":
                case "btnRight":
                case "btnDown":
                    // Define a mapping between button IDs and directions
                    var directionMap = new Dictionary<string, string>
                    {
                        { "btnLeft", "west" },
                        { "btnForward", "north" },
                        { "btnRight", "east" },
                        { "btnDown", "south" }
                    };
                    // Get the channel name or ID
                    var channelName = component.Channel.Name;

                    // Retrieve the document associated with the channel
                    var roomDoc = mongoDBConnection.selectDocument("rooms", "name", channelName);

                    if (roomDoc != null)
                    {
                        Database.Path room = BsonSerializer.Deserialize<Database.Path>(roomDoc);

                        //Get the direction that was clicked
                        string direction = directionMap.GetValueOrDefault(component.Data.CustomId);
                        var nextRoomName = direction switch
                        {
                            "west" => room.westRoom,
                            "north" => room.northRoom,
                            "east" => room.eastRoom,
                            "south" => room.southRoom,
                            _ => null
                        };

                        // Get the role that matches the next room name
                        var matchingRoleForNextRoom = user.Guild.Roles.FirstOrDefault(r => r.Name.Equals(nextRoomName, StringComparison.OrdinalIgnoreCase));
                        // Get the role that matches the current channel name
                        var matchingRoleForCurrentChannel = user.Guild.Roles.FirstOrDefault(r => r.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
                        
                        if (matchingRoleForCurrentChannel != null)
                        {
                            // Remove the current role from the user
                            await user.RemoveRoleAsync(matchingRoleForCurrentChannel);
                        }

                        if (matchingRoleForNextRoom != null)
                        {
                            // Add the next room role to the user
                            await user.AddRoleAsync(matchingRoleForNextRoom);
                            var rooms = user.Guild.TextChannels;
                            await rooms.FirstOrDefault(room => room.Name == matchingRoleForNextRoom.Name).SendMessageAsync($"{user.Mention} has entered the Room!");
                            await component.DeferAsync();
                        }
                        else
                        {
                            await component.RespondAsync($"No matching role found for the next room: {nextRoomName}.", ephemeral: true);
                        }

                    }
                    else
                    {
                        await component.RespondAsync("No room data found for this channel.", ephemeral: true);
                    }
                    break;
                    #endregion
            }
            return;
        }

        public static async Task enterDungeon(SocketGuildUser user)
        {
            try
            {
                await user.RemoveRoleAsync(user.Guild.Roles.FirstOrDefault(r => r.Name == "starter"));
                await user.AddRoleAsync(user.Guild.Roles.FirstOrDefault(r => r.Name == "room-1"));

                var rooms = user.Guild.TextChannels;
                await rooms.FirstOrDefault(room => room.Name == "room-1").SendMessageAsync($"{user.Mention} has entered the Dungeon!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
