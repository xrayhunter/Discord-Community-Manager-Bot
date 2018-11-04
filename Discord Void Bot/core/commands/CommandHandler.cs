using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord_Void_Bot.core.data;

namespace Discord_Void_Bot.core.commands
{
    class CommandHandler
    {
        private DiscordSocketClient client;
        private CommandService service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            this.client = client;
            this.service = new CommandService();

            await service.AddModulesAsync(Assembly.GetEntryAssembly());

            client.MessageReceived += Client_MessageReceived;
        }

        private async Task Client_MessageReceived(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(client, msg);
            
            if (context.User.IsBot) return;
            var account = UserAccounts.GetAccount(context.User);
            

            // Private stuff
            if (IsPrivateMessage(msg))
            {
                Console.WriteLine(msg.Author + " -> " + msg.Content);
            }
            else
            {

                // Muted stuff...
                if (account.IsMuted)
                {
                    await context.Message.DeleteAsync();
                    return;
                }

                if (account.LastMessage != msg.Content)
                {
                    if (!IsCommand(msg))
                    {
                        Console.WriteLine($"{context.User.Username} has a new message: {context.Message.Content}!");
                        account.LastMessage = msg.Content;
                        account.LastMessageID = msg.Id;
                        account.CountOfLastMessage = 0;
                    }

                    // Leveling stuff.
                    uint oldLevel = account.Level;
                    // Gives XP per word * 15 per attachment.
                    account.XP += (uint)(msg.Content.Split(' ').Length + (15 * msg.Attachments.Count));
                    UserAccounts.SaveAccounts();
                    uint newLevel = account.Level;

                    if (oldLevel != newLevel)
                    {
                        var embed = new EmbedBuilder();
                        embed.WithColor(Color.Green);
                        embed.WithTitle("LEVELED UP!");
                        embed.WithDescription(context.User.Username + $" just leveled up from {oldLevel}!");
                        embed.AddInlineField("LEVEL", newLevel);
                        embed.AddInlineField("XP", account.XP);

                        await context.Channel.SendMessageAsync("", embed: embed);
                    }
                }
                else
                {
                    if (!IsCommand(msg))
                    {
                        Console.WriteLine($"{context.User.Username} has repeated his/hers last message!");
                        account.CountOfLastMessage++;
                        await msg.DeleteAsync();
                    }

                    if (account.CountOfLastMessage < 10)
                    {
                        var orginialMessage = await context.Channel.GetMessageAsync(account.LastMessageID);
                        await orginialMessage.DeleteAsync();

                        var embed = new EmbedBuilder();
                        embed.WithColor(Color.DarkGrey);
                        embed.WithTitle(context.User.Username);
                        embed.WithDescription(account.LastMessage + "`x" + (account.CountOfLastMessage + 1) + "`");

                        orginialMessage = await context.Channel.SendMessageAsync("", embed: embed);
                        account.LastMessageID = orginialMessage.Id;
                    }
                    else
                    {
                        await AdminBot.Warn(context, (IGuildUser)context.User, $"Spamming chat in {context.Guild.Name}");
                    }
                }

                // Agree To TOS
                if (msg.Channel.Id == config.Config.configurations.welcomeChannelID && msg.Content != "-agree")
                {
                    await msg.DeleteAsync();
                    return;
                }
            }

            // Commands...
            int argPos = 0;
            if (IsCommand(msg, ref argPos))
            {
                if(!IsPrivateMessage(msg))
                {
                    await context.Message.DeleteAsync();
                    var result = await service.ExecuteAsync(context, argPos);

                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(result.ErrorReason);
                    }

                    if (!result.IsSuccess)
                    {
                        int delay = 5 * 1000;
                        string error;
                        RestUserMessage _msg;
                        LogSeverity severity = LogSeverity.Error;
                        var embed = new EmbedBuilder();
                        if (result.Error == CommandError.UnknownCommand)
                        {
                            embed.WithColor(Color.Green);
                            embed.WithTitle("Command Error");
                            error = $"{context.User.Mention}\nInvalid command `" + context.Message + "`!";
                            embed.WithDescription(error + $"\n\nMessage will be removed in {delay / 1000} seconds");
                        }
                        else if (result.Error == CommandError.BadArgCount)
                        {
                            embed.WithColor(Color.Green);
                            embed.WithTitle("Command Error");
                            error = $"{context.User.Mention}\nValid command `" + context.Message + "`, but incorrect arguments!";
                            embed.WithDescription(error + $"\n\nMessage will be removed in {delay / 1000} seconds");
                        }
                        else
                        {
                            embed.WithColor(Color.Green);
                            embed.WithTitle("System Failure :: Command");
                            error = $"{context.User.Mention}\nError has occured on an command `" + context.Message + "`, exception:\n" + result.ErrorReason;
                            embed.WithDescription(error + $"\n\nMessage will be removed in {delay / 1000} seconds");

                            severity = LogSeverity.Critical;
                        }

                        _msg = await context.Channel.SendMessageAsync("", embed: embed);
                        Logger.LogError(error, severity);
                        await Task.Delay(delay);
                        await _msg.DeleteAsync();
                    }
                }
                else
                {
                    Console.WriteLine("Private Command detected -> " + context.Message);
                    var result = await service.ExecuteAsync(context, argPos);
                    var dmChannel = await context.User.GetOrCreateDMChannelAsync();

                    if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(result.ErrorReason);
                    }

                    if (!result.IsSuccess)
                    {
                        int delay = 5 * 1000;
                        string error;
                        IUserMessage _msg;
                        LogSeverity severity = LogSeverity.Error;
                        var embed = new EmbedBuilder();
                        if (result.Error == CommandError.UnknownCommand)
                        {
                            embed.WithColor(Color.Green);
                            embed.WithTitle("Command Error");
                            error = $"{context.User.Mention}\nInvalid command `" + context.Message + "`!";
                            embed.WithDescription(error + $"\n\nMessage will be removed in {delay / 1000} seconds");
                        }
                        else if (result.Error == CommandError.BadArgCount)
                        {
                            embed.WithColor(Color.Green);
                            embed.WithTitle("Command Error");
                            error = $"{context.User.Mention}\nValid command `" + context.Message + "`, but incorrect arguments!";
                            embed.WithDescription(error + $"\n\nMessage will be removed in {delay / 1000} seconds");
                        }
                        else
                        {
                            embed.WithColor(Color.Green);
                            embed.WithTitle("System Failure :: Command");
                            error = $"{context.User.Mention}\nError has occured on an command `" + context.Message + "`, exception:\n" + result.ErrorReason;
                            embed.WithDescription(error + $"\n\nMessage will be removed in {delay / 1000} seconds");

                            severity = LogSeverity.Critical;
                        }

                        _msg = await dmChannel.SendMessageAsync("", embed: embed);
                        Logger.LogError(error, severity);
                        await Task.Delay(delay);
                        await _msg.DeleteAsync();
                    }
                }
            }
        }

        private bool IsCommand(SocketUserMessage msg)
        {
            int argPos = 0;
            return IsCommand(msg, ref argPos);
        }
        private bool IsCommand(SocketUserMessage msg, ref int argPos)
        {
            return msg.HasStringPrefix(config.Config.configurations.cmdPrefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos) || IsCommand(msg.Content, ref argPos);
        }
        private bool IsCommand(string msg, ref int argPos)
        {
            argPos = msg.IndexOf(config.Config.configurations.cmdPrefix);
            return msg.StartsWith(config.Config.configurations.cmdPrefix);
        }

        private bool IsPrivateMessage(SocketMessage msg)
        {
            return (msg.Channel.GetType() == typeof(SocketDMChannel));
        }
    }
}
