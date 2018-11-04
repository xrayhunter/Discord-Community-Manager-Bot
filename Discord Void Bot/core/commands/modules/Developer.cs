using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.core.commands.modules
{
    public class Developer : ModuleBase<SocketCommandContext>
    {
        [Command("setup")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task SetUp(bool shouldContinue = false, bool shouldCreateRoles = false, ITextChannel welcomeChannel = null, ITextChannel logs = null, ITextChannel mutedCh = null)
        {
            if (shouldContinue)
            {
                if (shouldCreateRoles)
                {
                    var muted = await Context.Guild.CreateRoleAsync("Muted");
                    config.Config.configurations.MutedID = muted.Id;
                    var member = await Context.Guild.CreateRoleAsync("Member");
                    config.Config.configurations.MemberID = member.Id;
                    var trusted = await Context.Guild.CreateRoleAsync("Trusted");
                    config.Config.configurations.TrustedID = trusted.Id;
                }

                if (welcomeChannel != null)
                {
                    config.Config.configurations.welcomeChannelID = welcomeChannel.Id;

                    var ch = (ITextChannel)Context.Guild.GetChannel(welcomeChannel.Id);

                    var post = new EmbedBuilder();
                    post.WithTitle(":flag_us: Welcome | :flag_fr: Bienvenue");
                    post.WithDescription($@"
                    :busts_in_silhouette: 
                    :flag_us: 
                    Welcome to {Context.Guild.Name}.
                    If you would like to join our community, please agree to our Terms and Conditions, Rules and Policies.
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    If you do agree, type `-agree`. Otherwise you will not receive full access to our discord.

                    Thank you, and have a great day. 
                    ==================================================
                    :flag_fr: 
                    Bienvenue à {Context.Guild.Name}.
                    Si vous aimeriez vous joindre à notre communauté, Nous vous demandons d'accepter nos termes et conditions, Règles et Politiques.
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    Si vous ete d’accord, Écrivez `-agree`. Ou vous n'allez pas avoir l'acces complet a notre discord. 

                    Merci, et passé une bonne journée.

                    ~ {Context.Guild.Name}'s Staff Team
                    ");
                    post.WithColor(new Color(0, 255, 0));

                    await ch.SendMessageAsync("", embed: post);
                }

                if (logs != null)
                {
                    config.Config.configurations.logsChannelID = logs.Id;
                }

                if (mutedCh != null)
                {
                    config.Config.configurations.mutedChannelID = mutedCh.Id;
                    var ch = (ITextChannel)Context.Guild.GetChannel(mutedCh.Id);

                    var post = new EmbedBuilder();
                    post.WithTitle(":flag_us: Welcome | :flag_fr: Bienvenue");
                    post.WithDescription($@"
                    :busts_in_silhouette: 
                    :flag_us: 
                    You were pushed to the Muted Section of the community, due to your failure to follow the Terms and Conditions, Rules and Policies
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing
                    ==================================================
                    :flag_fr: 
                    Vous avez été pousser en direction de la section muete de la communauté, Puisque vous n'avez pas respecter nos termes et condidtions, règles et politiques
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    ~ {Context.Guild.Name}'s Staff Team
                    ");
                    post.WithColor(new Color(0, 255, 0));

                    await ch.SendMessageAsync("", embed: post);
                }

                config.Config.Save();

                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

                var embed = new EmbedBuilder();
                embed.WithTitle("Notifying -> " + Context.User.Username);
                embed.WithDescription("Successfully setup the bot. Now you will need to setup the permissions for the new groups that have been given.");
                embed.WithColor(new Color(0, 255, 0));

                var msg = await Context.Channel.SendMessageAsync("", false, embed);

                int delay = (int)(config.Config.configurations.deletionDelay * 2) * 1000;
                await Task.Delay(delay);
                await msg.DeleteAsync();
            }
        }
        [Command("setup")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetUp(bool shouldContinue = false, bool shouldCreateRoles = false, bool shouldCreateWelcomeChannel = false, bool shouldCreateLogsChannel = false, bool shouldCreateMutedChannel = false)
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            if (shouldContinue)
            {
                if (shouldCreateRoles)
                {
                    var muted = await Context.Guild.CreateRoleAsync("Muted");
                    config.Config.configurations.MutedID = muted.Id;
                    var member = await Context.Guild.CreateRoleAsync("Member");
                    config.Config.configurations.MemberID = member.Id;
                    var trusted = await Context.Guild.CreateRoleAsync("Trusted");
                    config.Config.configurations.TrustedID = trusted.Id;
                }

                if (shouldCreateWelcomeChannel)
                {
                    var ch = await Context.Guild.CreateTextChannelAsync("newcomers");
                    config.Config.configurations.welcomeChannelID = ch.Id;

                    var post = new EmbedBuilder();
                    post.WithTitle(":flag_us: Welcome | :flag_fr: Bienvenue");
                    post.WithDescription($@"
                    :busts_in_silhouette: 
                    :flag_us: 
                    Welcome to {Context.Guild.Name}.
                    If you would like to join our community, please agree to our Terms and Conditions, Rules and Policies.
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    If you do agree, type `-agree`. Otherwise you will not receive full access to our discord.

                    Thank you, and have a great day. 
                    ==================================================
                    :flag_fr: 
                    Bienvenue à {Context.Guild.Name}.
                    Si vous aimeriez vous joindre à notre communauté, Nous vous demandons d'accepter nos termes et conditions, Règles et Politiques.
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    Si vous ete d’accord, Écrivez `-agree`. Ou vous n'allez pas avoir l'acces complet a notre discord. 

                    Merci, et passé une bonne journée.
                    ~ {Context.Guild.Name}'s Staff Team
                    ");
                    post.WithColor(new Color(0, 255, 0));

                    await ch.SendMessageAsync("", embed: post);
                }

                if (shouldCreateWelcomeChannel)
                {
                    var ch = await Context.Guild.CreateTextChannelAsync("logs");
                    config.Config.configurations.logsChannelID = ch.Id;
                }

                if (shouldCreateMutedChannel)
                {
                    var ch = await Context.Guild.CreateTextChannelAsync("muted");
                    config.Config.configurations.welcomeChannelID = ch.Id;

                    var post = new EmbedBuilder();
                    post.WithTitle(":flag_us: Welcome | :flag_fr: Bienvenue");
                    post.WithDescription($@"
                    :busts_in_silhouette: 
                    :flag_us: 
                    You were pushed to the Muted Section of the community, due to your failure to follow the Terms and Conditions, Rules and Policies
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing
                    ==================================================
                    :flag_fr: 
                    Vous avez été pousser en direction de la section muete de la communauté, Puisque vous n'avez pas respecter nos termes et condidtions, règles et politiques
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    ~ {Context.Guild.Name}'s Staff Team
                    ");
                    post.WithColor(new Color(0, 255, 0));

                    await ch.SendMessageAsync("", embed: post);
                }

                config.Config.Save();

                var embed = new EmbedBuilder();
                embed.WithTitle("Notifying -> " + Context.User.Username);
                embed.WithDescription("Successfully setup the bot. Now you will need to setup the permissions for the new groups that have been given.");
                embed.WithColor(new Color(0, 255, 0));

                await dmChannel.SendMessageAsync("", false, embed);
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithTitle("Notifying -> " + Context.User.Username);
                embed.WithDescription(":no_entry_sign: Setup cancled.");
                embed.WithColor(new Color(0, 255, 0));

                await dmChannel.SendMessageAsync("", false, embed);
            }
        }

        [Command("SetLogs")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetLogs(ITextChannel channel)
        {
            config.Config.configurations.logsChannelID = channel.Id;
        }

        [Command("SetMuted")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetMuted(ITextChannel channel)
        {
            config.Config.configurations.mutedChannelID = channel.Id;
            var ch = (ITextChannel)Context.Guild.GetChannel(channel.Id);

            var post = new EmbedBuilder();
            post.WithTitle(":flag_us: Welcome | :flag_fr: Bienvenue");
            post.WithDescription($@"
                    :busts_in_silhouette: 
                    :flag_us: 
                    You were pushed to the Muted Section of the community, due to your failure to follow the Terms and Conditions, Rules and Policies
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing
                    ==================================================
                    :flag_fr: 
                    Vous avez été pousser en direction de la section muete de la communauté, Puisque vous n'avez pas respecter nos termes et condidtions, règles et politiques
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    ~ {Context.Guild.Name}'s Staff Team
                    ");
            post.WithColor(new Color(0, 255, 0));

            await ch.SendMessageAsync("", embed: post);
        }

        [Command("SetNewComers")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetNewComers(ITextChannel channel)
        {
            config.Config.configurations.welcomeChannelID = channel.Id;

            var ch = (ITextChannel)Context.Guild.GetChannel(channel.Id);

            var post = new EmbedBuilder();
            post.WithTitle(":flag_us: Welcome | :flag_fr: Bienvenue");
            post.WithDescription($@"
                    :busts_in_silhouette: 
                    :flag_us: 
                    Welcome to {Context.Guild.Name}.
                    If you would like to join our community, please agree to our Terms and Conditions, Rules and Policies.
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    If you do agree, type `-agree`. Otherwise you will not receive full access to our discord.

                    Thank you, and have a great day. 
                    ==================================================
                    :flag_fr: 
                    Bienvenue à {Context.Guild.Name}.
                    Si vous aimeriez vous joindre à notre communauté, Nous vous demandons d'accepter nos termes et conditions, Règles et Politiques.
                    https://docs.google.com/document/d/1qdLr1OokyClw7quE-WzaG2oMg4tZLx-vLUmkKi9CIMw/edit?usp=sharing

                    Si vous ete d’accord, Écrivez `-agree`. Ou vous n'allez pas avoir l'acces complet a notre discord. 

                    Merci, et passé une bonne journée.

                    ~ {Context.Guild.Name}'s Staff Team
                    ");
            post.WithColor(new Color(0, 255, 0));

            await ch.SendMessageAsync("", embed: post);
        }

        [Command("echo")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Echo([Remainder]string msg)
        {
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("devinvite")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DevInvite()
        {
            await Context.Channel.SendMessageAsync("https://discordapp.com/oauth2/authorize?&client_id=475496390131318795&scope=bot&permissions=8");
        }
    }
}
