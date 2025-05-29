using Discord.WebSocket;

namespace DungeonMaster.Commands
{
    internal class CommandHandler
    {
        private readonly PlayerCommands _playerCommands;
        private readonly StaffCommands _staffCommands;

        public CommandHandler()
        {
            _playerCommands = new PlayerCommands();
            _staffCommands = new StaffCommands();
        }

        public async Task SlashCommandHandler(SocketSlashCommand command)
        {
            try
            {
                switch (command.CommandName)
                {
                    case "info":
                        await _playerCommands.HandleInfoCommand(command);
                        break;

                    case "status":
                        await _playerCommands.HandleStatusCommand(command);
                        break;

                    case "investigate":
                        await _playerCommands.HandleInvestigateCommand(command);
                        break;

                    #region Staff Commands
                    case "create-buttons":
                        if (isStaff(command.User as SocketGuildUser))
                            await _staffCommands.HandleCreateButtonCommand(command);
                        else
                            await command.RespondAsync("Only staff can use this command");
                        break;

                    case "create-item":
                        if (isStaff(command.User as SocketGuildUser))
                            await _staffCommands.HandleCreateItemCommand(command);
                        else
                            await command.RespondAsync("Only staff can use this command");
                        break;

                    case "create-path":
                        if (isStaff(command.User as SocketGuildUser))
                            await _staffCommands.HandleCreatePathCommand(command);
                        else
                            await command.RespondAsync("Only staff can use this command");
                        break;

                    case "create-message":
                        if (isStaff(command.User as SocketGuildUser))
                            await _staffCommands.HandleCreateMessageCommand(command);
                        else
                            await command.RespondAsync("Only staff can use this command");
                        break;

                    case "create-room":
                        if (isStaff(command.User as SocketGuildUser))
                            await _staffCommands.HandleCreateRoomCommand(command);
                        else
                            await command.RespondAsync("Only staff can use this command");
                        break;
                    #endregion
                    default:
                        await command.RespondAsync("Unknown command");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static bool isStaff(SocketGuildUser user)
        {
            if (user == null)
                return false;

            var roles = user.Roles;
            return roles.FirstOrDefault(sr => sr.Name == "Staff") != null;
        }
    }
}
