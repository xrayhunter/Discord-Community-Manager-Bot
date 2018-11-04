using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.core
{
    public class Logger
    {
        private static DiscordSocketClient client;
        private static config.BotConfiguration cfg;

        public async Task InitializeAsync(DiscordSocketClient _client, config.BotConfiguration _cfg)
        {
            client = _client;
            cfg = _cfg;
        }

        public async static void LogWarning(string msg)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Warning :warning:");
            embed.WithDescription(msg);
            embed.WithColor(new Color(255, 102, 26));

            var ch = GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        public async static void LogError(string msg, LogSeverity severity)
        {
            if (severity == LogSeverity.Info)
            {
                LogInfo(msg);
                return;
            }
            else if (severity == LogSeverity.Warning)
            {
                LogWarning(msg);
                return;
            }

            var embed = new EmbedBuilder();
            embed.WithTitle($"[ Logs ] :: {severity.ToString()} :exclamation: ");
            embed.WithDescription(msg);
            embed.WithColor(new Color(255, 0, 0));

            var ch = GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        public async static void LogInfo(string msg)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("[ Logs ] :: Info :information_source: ");
            embed.WithDescription(msg);
            embed.WithColor(new Color(0, 0, 255));

            var ch = GetLogChannel();
            await ch.SendMessageAsync("", embed: embed);
        }

        public static ITextChannel GetLogChannel()
        {
            return (ITextChannel)client.GetChannel(cfg.logsChannelID);
        }
    }
}
