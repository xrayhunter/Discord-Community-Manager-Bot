using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Void_Bot.core.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.core.commands.modules
{
    public class Administrative : ModuleBase<SocketCommandContext>
    {
        [Command("nuke")]
        [Name("nuke <amount>")]
        [Summary("Deletes a set amount of messages.")]
        public async Task Nuke([Remainder]string msg)
        {
            try
            {
                string[] args = msg.Split(' ');
                
                if (args.Length == 1)
                {
                    int value = Convert.ToInt32(args[0]);

                    var messages = await Context.Channel.GetMessagesAsync(value + 1).Flatten();
                    await Context.Channel.DeleteMessagesAsync(messages);

                    int delay = (int)config.Config.configurations.deletionDelay * 1000;
                    var response = await ReplyAsync($":white_check_mark: Success!\n Nuked was completed, this message will auto cleanup in {delay / 1000} seconds!");

                    Logger.LogInfo($":white_check_mark: Success!\n Nuked was executed at {Context.Channel.Name} by {Context.User.Username}. Count: {value}!");

                    await Task.Delay(delay);
                    await response.DeleteAsync();
                }
                else
                {
                    // Invalid Argument length.
                }
            }
            catch(Exception ex)
            {
                // Convertion issue.
            }
        }
        
        [Command("kick")]
        [Name("kick <mention> <reason>")]
        [Summary("A smarter way of kicking users.")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            await AdminBot.Kick(Context, user, reason);
        }

        [Command("ban")]
        [Name("ban <mention> <timelimitInDays> <reason>")]
        [Summary("A smarter way of banning users.")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser user, int howlong = 5, [Remainder]string reason = "No reason provided.")
        {
            await AdminBot.Ban(Context, user, howlong, reason);
        }

        [Command("warn")]
        [Name("warn <mention> <reason>")]
        [Summary("Warn a user.")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Warn(IGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            await AdminBot.Warn(Context, user, reason);
        }

        [Command("clearwarns")]
        [Name("clearwarns <mention>")]
        [Summary("Clears warnings/infractions from a user.")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task ClearWarns(IGuildUser user, int amount = -1)
        {
            var account = UserAccounts.GetAccount((SocketUser)user);

            if (amount == -1)
            {
                account.Warnings.Clear();
            }
            else
            {
                account.Warnings.RemoveRange(account.Warnings.Count - amount, amount);
            }
            UserAccounts.SaveAccounts();

            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string message = $@"
                :warning:
                Hello {user.Username}, all your warnings/infractions have been cleared by: {Context.User.Username}

                ~ {user.Guild.Name}'s Staff Team.
            ";

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(message);
            embed.WithColor(new Color(255, 0, 0));

            await dmChannel.SendMessageAsync("", false, embed);

            embed = new EmbedBuilder();
            embed.WithTitle("Notifying -> " + Context.User.Mention);
            int delay = (int)(config.Config.configurations.deletionDelay * 2) * 1000;
            if (amount == -1)
                embed.WithDescription($"{user.Username} was cleared from warnings/infractions in the discord by {Context.User.Username}!\nThis message will be removed in {delay / 1000} seconds!");
            else
                embed.WithDescription($"{user.Username} was cleared from {amount} warnings/infractions in the discord by {Context.User.Username}!\nThis message will be removed in {delay / 1000} seconds!");
            embed.WithColor(new Color(0, 255, 0));
            var msg = await Context.Channel.SendMessageAsync("", false, embed);

            if (amount == -1)
                Logger.LogInfo($":warning:{user.Username} was cleared from warnings/infractions in the discord by {Context.User.Username}!\n");
            else
                Logger.LogInfo($":warning:{user.Username} was cleared from {amount} warnings/infractions in the discord by {Context.User.Username}!\n");

            await Task.Delay(delay);
            await msg.DeleteAsync();
        }

        [Command("mute")]
        [Name("mute <mention> <reason>")]
        [Summary("Mute a user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task Mute(IGuildUser user, [Remainder]string reason = "No reason provided.")
        {
            await AdminBot.Mute(Context, user, reason);
        }

        [Command("unmute")]
        [Name("unmute <mention>")]
        [Summary("Unmute a user.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        public async Task UnMute(IGuildUser user)
        {
            var account = UserAccounts.GetAccount((SocketUser)user);
            account.IsMuted = false;

            IRole role = Context.Guild.EveryoneRole;
            foreach (IRole r in Context.Guild.Roles)
            {
                if (r.Id == config.Config.configurations.MutedID)
                    role = r;
            }

            await user.RemoveRoleAsync(role);
            UserAccounts.SaveAccounts();

            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string message = $@"
                :warning:
                Hello {user.Username}, you have been muted at {user.Guild.Name}!
                UnMuter: {Context.User.Mention} 

                ~ {user.Guild.Name}'s Staff Team.
            ";

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(message);
            embed.WithColor(new Color(255, 0, 0));
            await dmChannel.SendMessageAsync("", false, embed);

            embed = new EmbedBuilder();
            embed.WithTitle("Notifying -> " + Context.User.Mention);
            int delay = (int)(config.Config.configurations.deletionDelay * 2) * 1000;
            embed.WithDescription($"{user.Username} was unmuted from the discord by {Context.User.Username}!\nThis message will be removed in {delay / 1000} seconds!");
            embed.WithColor(new Color(0, 255, 0));
            var msg = await Context.Channel.SendMessageAsync("", false, embed);

            await Task.Delay(delay);
            await msg.DeleteAsync();
        }

        [Command("inspect")]
        [Name("inspect <mention>")]
        [Summary("Inspects the player's account.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Inspect(IGuildUser user, bool showWarnings = false, bool showKicks = false, bool showBans = false)
        {
            var account = UserAccounts.GetAccount((SocketUser)user);

            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            string message = $@"
                :information_source: :spy: 
                Real Username: {user.Username}
                Discord Profile: {user.Mention}
                
                :robot: 
                Is Bot: {user.IsBot}
                Is Webhook: {user.IsWebhook}
                

                :calendar: 
                Creation Date: {user.CreatedAt}
                First Joined On: {account.FirstJoinDate}
                
                :warning: 
                Active Warnings: {account.Warnings.Count}
                Kicks: {account.Kicks.Count}
                Bans: {account.Bans.Count}

                :speaking_head: 
                Is Muted: {account.IsMuted}
            ";

            var embed = new EmbedBuilder();
            embed.WithTitle(user.Username + "'s Profile:");
            embed.WithDescription(message);
            embed.WithColor(new Color(255, 0, 0));

            await dmChannel.SendMessageAsync("", false, embed);

            if (showWarnings)
            {
                foreach (ActionReport report in account.Warnings)
                {
                    message = $@"
                        :information_source: :spy: :warning: 
                        Staff Name: {report.actionerName}
                        Reason: {report.reason}
                        Given Date: {report.date}
                    ";

                    embed = new EmbedBuilder();
                    embed.WithTitle("[Action Report | Warning] :: " +user.Username + "'s Profile:");
                    embed.WithDescription(message);
                    embed.WithColor(new Color(255, 0, 0));

                    await dmChannel.SendMessageAsync("", false, embed);
                }
            }

            if (showKicks)
            {
                foreach (ActionReport report in account.Kicks)
                {
                    message = $@"
                        :information_source: :spy: :boot: 
                        Staff Name: {report.actionerName}
                        Reason: {report.reason}
                        Given Date: {report.date}
                    ";

                    embed = new EmbedBuilder();
                    embed.WithTitle("[Action Report | Kick] :: " + user.Username + "'s Profile:");
                    embed.WithDescription(message);
                    embed.WithColor(new Color(255, 0, 0));

                    await dmChannel.SendMessageAsync("", false, embed);
                }
            }

            if (showKicks)
            {
                foreach (ActionReport report in account.Kicks)
                {
                    message = $@"
                        :information_source: :spy: :hammer: 
                        Staff Name: {report.actionerName}
                        Reason: {report.reason}
                        Given Date: {report.date}
                        How long: {report.howLong}
                    ";

                    embed = new EmbedBuilder();
                    embed.WithTitle("[Action Report | Ban] :: " + user.Username + "'s Profile:");
                    embed.WithDescription(message);
                    embed.WithColor(new Color(255, 0, 0));

                    await dmChannel.SendMessageAsync("", false, embed);
                }
            }

            embed = new EmbedBuilder();
            embed.WithTitle("Notifying -> " + Context.User.Mention);
            int delay = (int)(config.Config.configurations.deletionDelay * 2) * 1000;
            embed.WithDescription($"Check your DMs, I'm sending my secrets. :P\nThis message will be removed in {delay / 1000} seconds!");
            embed.WithColor(new Color(0, 255, 0));

            var msg = await Context.Channel.SendMessageAsync("", false, embed);

            await Task.Delay(delay);
            await msg.DeleteAsync();

        }
    }
}
