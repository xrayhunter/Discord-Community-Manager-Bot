using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Void_Bot.core.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.core
{
    public static class AdminBot
    {
        public async static Task Ban(SocketCommandContext Context, IGuildUser user, int howlong = 5, string reason = "No reason provided.", bool isSystem = false)
        {
            string actioner = Context.User.Mention;
            if (isSystem)
                actioner = "AdminBot";

            var account = UserAccounts.GetAccount((SocketUser)user);
            account.Bans.Add(new ActionReport()
            {
                actionerName = actioner,
                actionerID = Context.User.Id,
                reason = reason,
                date = DateTime.Today.ToString(),
                howLong = howlong
            });
            UserAccounts.SaveAccounts();
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string message = $@"
                :warning:
                Hello {user.Username}, you have been banned from {user.Guild.Name}!
                Banner: {actioner} 
                Reason: {reason}
                Release: {howlong} days

                ~ {user.Guild.Name}'s Staff Team.
            ";

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(message);
            embed.WithColor(new Color(255, 0, 0));

            await dmChannel.SendMessageAsync("", false, embed);

            await Task.Delay(500);
            await user.Guild.AddBanAsync(user, howlong, reason);

            embed = new EmbedBuilder();
            embed.WithTitle("Notifying -> " + Context.User.Mention);
            int delay = (int)(config.Config.configurations.deletionDelay * 2) * 1000;
            embed.WithDescription($"{user.Username} was banned from the discord by {actioner} for {reason} :: {howlong} days!\nThis message will be removed in {delay / 1000} seconds!");
            embed.WithColor(new Color(0, 255, 0));

            Logger.LogInfo($"{user.Username} was banned from the discord by {actioner} for {reason} :: {howlong} days!\nThis message will be removed in {delay / 1000} seconds!");

            var msg = await Context.Channel.SendMessageAsync("", false, embed);

            await Task.Delay(delay);
            await msg.DeleteAsync();
        }

        public async static Task Kick(SocketCommandContext Context, IGuildUser user, string reason = "No reason provided.", bool isSystem = false)
        {
            string actioner = Context.User.Mention;
            if (isSystem)
                actioner = "AdminBot";

            var account = UserAccounts.GetAccount((SocketUser)user);
            account.Kicks.Add(new ActionReport()
            {
                actionerName = actioner,
                actionerID = Context.User.Id,
                reason = reason,
                date = DateTime.Today.ToString()
            });
            UserAccounts.SaveAccounts();
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string message = $@"
                :warning:
                Hello {user.Username}, you have been kicked from {user.Guild.Name}!
                Kicker: {actioner} 
                Reason: {reason}

                ~ {user.Guild.Name}'s Staff Team.
            ";

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(message);
            embed.WithColor(new Color(255, 0, 0));

            await dmChannel.SendMessageAsync("", false, embed);

            await Task.Delay(500);
            await user.KickAsync(reason);

            embed = new EmbedBuilder();
            embed.WithTitle("Notifying -> " + Context.User.Mention);
            int delay = (int)(config.Config.configurations.deletionDelay * 2) * 1000;
            embed.WithDescription($"{user.Username} was kicked from the discord by {actioner} for {reason}!\nThis message will be removed in {delay / 1000} seconds!");
            embed.WithColor(new Color(0, 255, 0));

            Logger.LogInfo($"{user.Username} was kicked from the discord by {actioner} for {reason}!\nThis message will be removed in {delay / 1000} seconds!");

            var msg = await Context.Channel.SendMessageAsync("", false, embed);

            await Task.Delay(delay);
            await msg.DeleteAsync();
        }

        public async static Task Mute(SocketCommandContext Context, IGuildUser user, string reason = "No reason provided.", bool isSystem = false)
        {
            string actioner = Context.User.Mention;
            if (isSystem)
                actioner = "AdminBot";

            var account = UserAccounts.GetAccount((SocketUser)user);
            account.IsMuted = true;

            IRole role = Context.Guild.EveryoneRole;
            foreach (IRole r in Context.Guild.Roles)
            {
                if (r.Id == config.Config.configurations.MutedID)
                    role = r;
            }

            await user.AddRoleAsync(role);
            UserAccounts.SaveAccounts();

            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string message = $@"
                :warning:
                Hello {user.Username}, you have been muted at {user.Guild.Name}!
                Muter: {actioner} 
                Reason: {reason}

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
            embed.WithDescription($"{user.Username} was muted from the discord by {actioner} for {reason}!\nThis message will be removed in {delay / 1000} seconds!");
            embed.WithColor(new Color(0, 255, 0));

            var msg = await Context.Channel.SendMessageAsync("", false, embed);

            await Task.Delay(delay);
            await msg.DeleteAsync();
        }

        public async static Task Warn(SocketCommandContext Context, IGuildUser user, string reason = "No reason provided.", bool isSystem = false)
        {
            string actioner = Context.User.Mention;
            if (isSystem)
                actioner = "AdminBot";

            var account = UserAccounts.GetAccount((SocketUser)user);
            account.Warnings.Add(new ActionReport()
            {
                actionerName = actioner,
                actionerID = Context.User.Id,
                reason = reason,
                date = DateTime.Today.ToString()
            });


            var dmChannel = await user.GetOrCreateDMChannelAsync();
            string message = $@"
                :warning:
                Hello {user.Username}, you have been warn for a infraction at {user.Guild.Name}!
                Warner: {actioner} 
                Reason: {reason}
                ~ { user.Guild.Name}'s Staff Team.
            ";

            if (account.Warnings.Count >= config.Config.configurations.banThreashHold && config.Config.configurations.banWarnThreshHold)
            {
                await Ban(Context, user, 1, reason);
            }
            else if (account.Warnings.Count >= config.Config.configurations.muteThreashHold && config.Config.configurations.muteWarnThreashHold)
            {
                await Mute(Context, user);
            }
            UserAccounts.SaveAccounts();

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(message);
            embed.WithColor(new Color(255, 0, 0));

            await dmChannel.SendMessageAsync("", false, embed);

            embed = new EmbedBuilder();
            embed.WithTitle("Notifying -> " + Context.User.Mention);
            int delay = (int)(config.Config.configurations.deletionDelay * 2) * 1000;
            embed.WithDescription($"{user.Username} was warned from the discord by {actioner} for {reason}!\nThis message will be removed in {delay / 1000} seconds!");
            embed.WithColor(new Color(0, 255, 0));

            Logger.LogInfo($"{user.Username} was warned from the discord by {actioner} for {reason}!\nThis message will be removed in {delay / 1000} seconds!");

            var msg = await Context.Channel.SendMessageAsync("", false, embed);

            await Task.Delay(delay);
            await msg.DeleteAsync();
        }
    }
}
