using Discord;
using System.Linq;
using Discord.Net;
using Discord.WebSocket;
using DungeonMaster;
using Newtonsoft.Json;
using System.Collections;
using System.Data;
using DungeonMaster.Database;
using DungeonMaster.Commands;
using MongoDB.Bson.Serialization;

public class Program
{
    private static DiscordSocketClient client;


    public static async Task Main()
    {
        CommandHandler commandHandler = new CommandHandler();

        Console.WriteLine("Connecting to Discord...");
        client = new DiscordSocketClient();

        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        var token = "discordBotAPIkeyGoesHere";

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        client.Ready += Client_Ready;

        client.SlashCommandExecuted += commandHandler.SlashCommandHandler;
        client.ButtonExecuted += ButtonHandler.ButtonActions;

        // Block this task until the program is closed.
        await Task.Delay(-1);

    }

    public static async Task Client_Ready()
    {
        // Let's do our global command
        //1347393785843417181 is the "Casey's Dungeon Crawl" Server

        Console.WriteLine("Do you want to refresh the Discord Server? Y/N");
        Console.WriteLine("Warning: This will purge all messages in the dungeon rooms");
        string userInput = Console.ReadLine()?.Trim().ToUpper();

        if (userInput == "Y")
        {
            await clearCommands(1347393785843417181);
            await loadCommands(1347393785843417181);
            //await deleteChannels(1347393785843417181);
            await confirmChannels(1347393785843417181);
        }
        else
            Console.WriteLine("Skipping Refresh");


        Console.WriteLine("Bot is connected and Ready!");
    }

    public static async Task clearCommands(ulong guildID)
    {
        // Clear guild-specific commands
        var guild = client.GetGuild(guildID);
        var guildCommands = await guild.GetApplicationCommandsAsync();
        Console.WriteLine($"Removing {guildCommands.Count} guild-specific commands...");
        await guild.DeleteApplicationCommandsAsync();

        // Clear global commands
        var globalCommands = await client.GetGlobalApplicationCommandsAsync();
        Console.WriteLine($"Removing {globalCommands.Count} global commands...");
        foreach (var command in globalCommands)
            await command.DeleteAsync();
    }


    public static async Task loadCommands(ulong guildID)
    {
        var guild = client.GetGuild(guildID);

        #region Commands
        ArrayList slashCommands = new ArrayList();

        slashCommands.Add(new SlashCommandBuilder().WithName("status").WithDescription("Provides information on yourself!"));

        slashCommands.Add(new SlashCommandBuilder().WithName("investigate").WithDescription("searches the current room for loot, events or monsters"));

        slashCommands.Add(new SlashCommandBuilder().WithName("info").WithDescription("Provides information on an item")
            .AddOption("item-name", ApplicationCommandOptionType.String, "The name of the item", true));

        slashCommands.Add(new SlashCommandBuilder().WithName("create-message").WithDescription("Creates a message in a specified channel")
            .AddOption("message", ApplicationCommandOptionType.String, "The message you want the bot to say", true)
            .AddOption("channel", ApplicationCommandOptionType.Channel, "The channel of where to place this message"));

        slashCommands.Add(new SlashCommandBuilder().WithName("create-buttons").WithDescription("Creates a customized button in a specified channel")
            .AddOption("channel", ApplicationCommandOptionType.Channel, "The channel of where to place these buttons", true));

        slashCommands.Add(new SlashCommandBuilder().WithName("create-room").WithDescription("Creates an room entry in the database so it can be re-generated")
            .AddOption("title", ApplicationCommandOptionType.String, "The title of the dungeon room", true)
            .AddOption("description", ApplicationCommandOptionType.String, "The description of the dungeon room", true)
            .AddOption("channel", ApplicationCommandOptionType.Channel, "The channel name to be made as a room", true));

        slashCommands.Add(new SlashCommandBuilder().WithName("create-item").WithDescription("Adds a barebones item into the database")
            .AddOption("name", ApplicationCommandOptionType.String, "The name of the item", true)
            .AddOption("description", ApplicationCommandOptionType.String, "The description of the item", true));

        slashCommands.Add(new SlashCommandBuilder().WithName("create-path").WithDescription("Adds a path into the database")
            .AddOption("room-id", ApplicationCommandOptionType.Channel, "The room you are leaving from", true)
            .AddOption("west", ApplicationCommandOptionType.Channel, "The room this path leads to... if any", false)
            .AddOption("north", ApplicationCommandOptionType.Channel, "The room this path leads to... if any", false)
            .AddOption("south", ApplicationCommandOptionType.Channel, "The room this path leads to... if any", false)
            .AddOption("east", ApplicationCommandOptionType.Channel, "The room this path leads to... if any", false)
            .AddOption("veer-west", ApplicationCommandOptionType.Channel, "The room this path leads to... if any", false)
            .AddOption("veer-east", ApplicationCommandOptionType.Channel, "The room this path leads to... if any", false));




        try
        {
            foreach (SlashCommandBuilder command in slashCommands)
            {
                Console.WriteLine("Adding slash command: " + command.Name);
                await guild.CreateApplicationCommandAsync(command.Build());
            }

        }
        catch(Exception e)
        {
            Console.WriteLine("Exception during loading commands: " + e.Message);
        }
        #endregion
    }

    /*public static async Task deleteChannels(ulong guildID)
    {
        var guild = client.GetGuild(guildID);
        var textChannels = guild.TextChannels;
        for (int i = 1; i < 51; i++)
        {
            if (textChannels.FirstOrDefault(tc => tc.Name == "room-" + i) != null)
            {
                Console.WriteLine("Deleting room-" + i);
                await textChannels.FirstOrDefault(tc => tc.Name == "room-" + i).DeleteAsync();
            }
        }
    }*/

    public static async Task confirmChannels(ulong guildID)
    {
        var guild = client.GetGuild(guildID);
        if (guild == null)
        {
            Console.WriteLine("Guild not found.");
            return;
        }

        var guildRoles = guild.Roles;
        var dungeonCategory = guild.CategoryChannels.FirstOrDefault(g => g.Name == "Dungeon")?.Id;

        if (dungeonCategory == null)
        {
            Console.WriteLine("Dungeon category not found.");
            return;
        }

        var existingTextChannels = guild.TextChannels.ToDictionary(tc => tc.Name, tc => tc);
        var existingRoles = guildRoles.ToDictionary(r => r.Name, r => r);

        for (int i = 1; i <= 50; i++) // Discord allows up to 50 text channels
        {
            string roomName = $"room-{i}";

            // Ensure the role exists
            if (!existingRoles.TryGetValue(roomName, out var roomRole))
            {
                Console.WriteLine($"Creating role: {roomName}");
                await guild.CreateRoleAsync(roomName);
            }

            // Ensure the text channel exists
            if (!existingTextChannels.ContainsKey(roomName))
            {
                Console.WriteLine($"Creating text chat: {roomName}");
                try
                {
                    // Define channel properties
                    var channelProperties = new Action<TextChannelProperties>(properties =>
                    {
                        properties.CategoryId = dungeonCategory;
                        properties.ChannelType = ChannelType.Text;
                        properties.Position = i + 2;

                        // Set permissions for the channel
                        properties.PermissionOverwrites = new List<Overwrite>
                        {
                            new Overwrite(guild.EveryoneRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Deny)),
                            new Overwrite(roomRole.Id, PermissionTarget.Role, new OverwritePermissions(viewChannel: PermValue.Allow))
                        };
                    });

                    // Create the text channel with predefined properties
                    await guild.CreateTextChannelAsync(roomName, channelProperties);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create text chat {roomName}: {ex.Message}");
                }
            }
            else
            {
                existingTextChannels.TryGetValue(roomName, out var textChannel);
                await purgeMessages(textChannel);
            }
        }
        existingTextChannels.TryGetValue("news-from-the-barker", out var newsChannel);
        await newsChannel.SendMessageAsync("The dungeon purged all the messages from its walls!");
    }

    public static async Task purgeMessages(SocketTextChannel dungeonRoom)
    {
        try
        {
            Console.WriteLine($"Deleting messages in channel: {dungeonRoom.Name}");

            // Retrieve messages in batches of 100 (Discord's limit)
            var messages = await dungeonRoom.GetMessagesAsync(100).FlattenAsync();

            while (messages.Any())
            {
                // Delete the messages in bulk
                await dungeonRoom.DeleteMessagesAsync(messages);

                Console.WriteLine($"Deleted {messages.Count()} messages in channel: {dungeonRoom.Name}");

                // Retrieve the next batch of messages
                messages = await dungeonRoom.GetMessagesAsync(100).FlattenAsync();
            }

            Console.WriteLine($"All messages deleted in channel: {dungeonRoom.Name}");
            await repopRoom(dungeonRoom);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to delete messages in channel {dungeonRoom.Name}: {ex.Message}");
        }

    }

    public static async Task repopRoom(SocketTextChannel dungeonRoom)
    {
        try
        {
            Console.WriteLine($"Repopulating room: {dungeonRoom.Name}");

            // Initialize MongoDB connection
            var mongoDBConnection = new MongoDBConnection();

            // Retrieve the room document from the database
            var roomDoc = mongoDBConnection.selectDocument("rooms", "name", dungeonRoom.Name);
            if (roomDoc == null)
            {
                Console.WriteLine($"Room {dungeonRoom.Name} not found in the database.");
                return;
            }

            // Deserialize the room document into a Room object
            var room = BsonSerializer.Deserialize<Room>(roomDoc);

            // Create an embed message using the room's title and description
            var embed = new EmbedBuilder()
                .WithTitle(room.title)
                .WithDescription(room.desc)
                .WithColor(Color.Teal) // Set the embed color
                .AddField("**Objective**", room.objective ?? "`/investigate` the room to help you decide your next step")
                .AddField("**Tip**", room.tip ?? "Use `/status` to check your stats and `/info` to learn about items.");

            // Send the embed message to the channel
            await dungeonRoom.SendMessageAsync(embed: embed.Build());

            // Check if the room has buttons
            if (room.buttons != null && room.buttons.Any())
            {
                // Create a ComponentBuilder for the buttons
                var componentBuilder = new ComponentBuilder();

                foreach (var button in room.buttons)
                {
                    // Add each button to the ComponentBuilder
                    componentBuilder.WithButton(button.label, button.customId, (ButtonStyle)button.style);
                }

                // Send the buttons as a separate message
                await dungeonRoom.SendMessageAsync("", components: componentBuilder.Build());
            }

            Console.WriteLine($"Room {dungeonRoom.Name} has been repopulated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to repopulate room {dungeonRoom.Name}: {ex.Message}");
        }
    }




}