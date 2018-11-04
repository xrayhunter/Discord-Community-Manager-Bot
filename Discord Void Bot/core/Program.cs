using Discord;
using Discord.WebSocket;
using Discord_Void_Bot.core.commands;
using Discord_Void_Bot.config;
using System.Threading.Tasks;
using System;
using Discord_Void_Bot.core.data;
using Discord_Void_Bot.core;

namespace Discord_Void_Bot
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandHandler cmdHandler;
        private Logger logger;

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.configurations.token == "" || Config.configurations.token == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[System Failure] :: Token was not set!");
                await Task.Delay(5*1000);
                return;
            }

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose
            });
            
            // Logging
            client.Connected += Client_Connected;
            client.Disconnected += Client_Disconnected;
            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;

            // Messages
            client.MessageDeleted += Client_MessageDeleted;

            // Bans
            client.UserBanned += Client_UserBanned;
            client.UserUnbanned += Client_UserUnbanned;

            // Channels
            client.ChannelCreated += Client_ChannelCreated;
            client.ChannelDestroyed += Client_ChannelDestroyed;
            client.ChannelUpdated += Client_ChannelUpdated;
            
            // Roles
            client.RoleCreated += Client_RoleCreated;
            client.RoleDeleted += Client_RoleDeleted;
            client.RoleUpdated += Client_RoleUpdated;
            
            client.Log += Client_Log;

            await client.LoginAsync(TokenType.Bot, Config.configurations.token);
            await client.StartAsync();

            logger = new Logger();
            await logger.InitializeAsync(client, Config.configurations);

            cmdHandler = new CommandHandler();
            await cmdHandler.InitializeAsync(client);
            await Task.Delay(-1);
        }

        private async Task Client_MessageDeleted(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription("Message was removed!");
            embed.AddField("Message", msg.Value.Content);
            embed.AddField("Author", msg.Value.Author);
            embed.WithColor(new Color(0, 0, 255));

            var ch = Logger.GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        private async Task Client_ChannelUpdated(SocketChannel oldChannel, SocketChannel newChannel)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription("Channel was updated!");
            embed.AddField("Old Channel", oldChannel.ToString());
            embed.AddField("New Channel", newChannel.ToString());
            embed.WithColor(new Color(0, 0, 255));

            var ch = Logger.GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        private async Task Client_ChannelDestroyed(SocketChannel channel)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription("Channel was destroyed!");
            embed.AddField("Channel", channel.ToString());
            embed.WithColor(new Color(0, 0, 255));

            var ch = Logger.GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        private async Task Client_ChannelCreated(SocketChannel channel)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription("Channel was created!");
            embed.AddField("Channel", channel.ToString());
            embed.WithColor(new Color(0, 0, 255));

            var ch = Logger.GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        private async Task Client_RoleUpdated(SocketRole oldRole, SocketRole updatedRole)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription("Role was updated!");
            embed.AddField("Old Role Name", oldRole.Name);
            embed.AddField("New Role Name", oldRole.Name);
            embed.WithColor(new Color(0, 0, 255));

            var ch = Logger.GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        private async Task Client_RoleDeleted(SocketRole role)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription("Role was deleted!");
            embed.AddField("Role", role.Name);
            embed.WithColor(new Color(0, 0, 255));

            var ch = Logger.GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        private async Task Client_RoleCreated(SocketRole role)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription("Role was created!");
            embed.AddField("Role", role.Name);
            embed.WithColor(new Color(0, 0, 255));

            var ch = Logger.GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        private async Task Client_UserUnbanned(SocketUser user, SocketGuild guild)
        {
            Logger.LogInfo($"{user.Username} was unbanned from {guild.Name}");

            Console.WriteLine(user.Username + " was unbanned from " + guild.Name);
            
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string greating = @"
                :hammer: :warning: 
                Hello %username%, you were unbanned from %servername%!
                Come and join us back at: %joinlink%

                Thank you and hope you join us back at %servername%
                ~ %servername%'s Staff Team
                ";
            if (Config.configurations.howToGreatPlayers != null || Config.configurations.howToGreatPlayers != "")
                greating = Config.configurations.howToGreatPlayers;

            var invite = await guild.DefaultChannel.CreateInviteAsync();

            greating.Replace("%username%", user.Username);
            greating.Replace("%servername%", guild.Name);
            greating.Replace("%joinlink%", invite.Url);

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(greating);
            embed.WithColor(new Color(0, 255, 0));

            await dmChannel.SendMessageAsync("", false, embed);
        }

        private async Task Client_UserBanned(SocketUser user, SocketGuild guild)
        {
            Logger.LogInfo($"{user.Username} was banned from {guild.Name}");

            Console.WriteLine(user.Username + " was banned from " + guild.Name);

            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string greating = @"
                :hammer: :warning: 
                Hello %username%, you were banned from an administrator at %servername%.
                If you wish to be unbanned, please appeal at %communitywebsite%
                Sincerely,
                ~ %servername%'s Staff Team
                ";
            if (Config.configurations.howToGreatPlayers != null || Config.configurations.howToGreatPlayers != "")
                greating = Config.configurations.howToGreatPlayers;

            greating.Replace("%username%", user.Username);
            greating.Replace("%servername%", guild.Name);
            greating.Replace("%communitywebsite%", Config.configurations.communityWebsite);

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(greating);
            embed.WithColor(new Color(0, 255, 0));

            await dmChannel.SendMessageAsync("", false, embed);
        }

        private async Task Client_Disconnected(Exception arg)
        {
            Console.WriteLine("Client disconnected! Exception: " + arg.Message);
        }

        private async Task Client_Connected()
        {
            Console.WriteLine("Client connected!");
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        {
            Logger.LogInfo($"{user.Username} has joined {user.Guild.Name}!");

            var account = UserAccounts.GetAccount(user);

            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string greating = @"
                :white_check_mark: 
                Welcome {0} to {1}! 
                If you need help, please type `help` either in here, or type `{2}help` or mention me then do `help` by it's self. 
                Thank you and have a great day :smile_cat:!
                ~ {1}'s Staff Team :stars:";
            if (Config.configurations.howToGreatPlayers != null || Config.configurations.howToGreatPlayers != "")
                greating = Config.configurations.howToGreatPlayers;

            greating = String.Format(greating, user.Username, user.Guild.Name, Config.configurations.cmdPrefix);

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(greating);
            embed.WithColor(new Color(0, 255, 0));

            await dmChannel.SendMessageAsync("", false, embed);
        }

        private async Task Client_UserLeft(SocketGuildUser user)
        {
            Logger.LogInfo($"{user.Username} has left {user.Guild.Name}!");

            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string greating = @"
                :white_check_mark: 
                Goodbye {0}! 
                We hope that you had a great time with our community, and we hope the best for you! :smile_cat:
                ~ {1}'s Staff Team :stars:";
            if (Config.configurations.howToGreatPlayers != null || Config.configurations.howToGreatPlayers != "")
                greating = Config.configurations.howToGreatPlayers;

            greating = String.Format(greating, user.Username, user.Guild.Name);

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(greating);
            embed.WithColor(new Color(0, 255, 0));

            await dmChannel.SendMessageAsync("", false, embed);
        }

        private async Task Client_Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
