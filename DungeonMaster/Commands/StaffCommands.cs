using Discord.WebSocket;
using Discord;
using DungeonMaster.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace DungeonMaster.Commands
{
    internal class StaffCommands
    {
        private readonly MongoDBConnection _mongoDBConnection;

        public StaffCommands()
        {
            _mongoDBConnection = new MongoDBConnection();
        }

        public async Task HandleCreateButtonCommand(SocketSlashCommand command)
        {
            IGuildChannel targetChannel = (IGuildChannel)command.Data.Options.FirstOrDefault(p => p.Name == "channel")?.Value;

            var roomDoc = _mongoDBConnection.selectDocument("rooms", "name", targetChannel.Name);
            Room room;

            if (roomDoc != null)
            {
                // Deserialize the existing room document
                room = BsonSerializer.Deserialize<Room>(roomDoc);

                var words = new List<string> { "Explore", "Journey", "Venture", "Pursue", "Advance", "Stride", "Surge", "Wander", "Adventure", "Tour", "Chart", "Navigate" };
                var random = new Random();

                room.buttons.Add(new Button($"◀️ Explore West", "btnWest", (ButtonStyle)1));
                room.buttons.Add(new Button($"🔼 Explore North", "btnNorth", (ButtonStyle)1));
                room.buttons.Add(new Button($"🔽 Explore South", "btnSouth", (ButtonStyle)1));
                room.buttons.Add(new Button($"▶️ Explore East", "btnEast", (ButtonStyle)1));

                // Upsert the updated room document into the database
                await _mongoDBConnection.upsertDocument("rooms", room.name, room.ToJson());
                await command.RespondAsync("Button has been added to " + room.name + " in the database");
            }
            else
                await command.RespondAsync("Targeted channel was not found in database");
        }

        public async Task HandleCreateItemCommand(SocketSlashCommand command)
        {
            string name = (string)command.Data.Options.First().Value;
            string description = (string)command.Data.Options.Skip(1).First().Value;

            Item item = new Item
            {
                name = name,
                desc = description
            };

            try
            {
                await _mongoDBConnection.upsertDocument("items", item.name, item.ToJson());
                await command.RespondAsync("Success!");
            }
            catch (Exception)
            {
                await command.RespondAsync("No good! :(");
            }
        }

        public async Task HandleCreatePathCommand(SocketSlashCommand command)
        {
            IGuildChannel targetChannel = (IGuildChannel)command.Data.Options.First().Value;

            Database.Path roomPath = new Database.Path();

            // Handle optional parameters
            var westOption = command.Data.Options.FirstOrDefault(p => p.Name == "west");
            roomPath.westRoom = westOption?.Value?.ToString(); // Assign null if not provided

            var eastOption = command.Data.Options.FirstOrDefault(p => p.Name == "east");
            roomPath.eastRoom = eastOption?.Value?.ToString();

            var northOption = command.Data.Options.FirstOrDefault(p => p.Name == "north");
            roomPath.northRoom = northOption?.Value?.ToString();

            var southOption = command.Data.Options.FirstOrDefault(p => p.Name == "south");
            roomPath.southRoom = southOption?.Value?.ToString();

            var veerWestOption = command.Data.Options.FirstOrDefault(p => p.Name == "veer-west");
            roomPath.veerWest = veerWestOption?.Value?.ToString();

            var veerEastOption = command.Data.Options.FirstOrDefault(p => p.Name == "veer-east");
            roomPath.veerEast = veerEastOption?.Value?.ToString();

            roomPath.name = targetChannel.Name;

            // Save the roomPath to the database
            await _mongoDBConnection.upsertDocument("paths", targetChannel.Name, roomPath.ToJson());
            await command.RespondAsync("Room paths updated!");
        }

        public async Task HandleCreateMessageCommand(SocketSlashCommand command)
        {
            string message = (string)command.Data.Options.First().Value;
            IGuildChannel targetChannel = (IGuildChannel)command.Data.Options.Skip(1).First().Value;

            if (targetChannel is ITextChannel textChannel)
            {
                await textChannel.SendMessageAsync(message);
                await command.RespondAsync($"A message has been created in <#{targetChannel.Id}>");
            }
            else
                await command.RespondAsync("Targeted channel is not a text channel");
        }

        public async Task HandleCreateRoomCommand(SocketSlashCommand command)
        {
            // Extract options from the command
            string title = (string)command.Data.Options.FirstOrDefault(p => p.Name == "title")?.Value;
            string description = (string)command.Data.Options.FirstOrDefault(p => p.Name == "description")?.Value;
            IGuildChannel targetChannel = (IGuildChannel)command.Data.Options.FirstOrDefault(p => p.Name == "channel")?.Value;

            // Create a new Room object
            Room room = new Room
            {
                name = targetChannel.Name,
                title = title,
                desc = description
            };

                // Save the room to the database
                await _mongoDBConnection.upsertDocument("rooms", room.name, room.ToJson());
                await command.RespondAsync($"Room '{room.name}' has been successfully added to the database!");
        }
    }

}

