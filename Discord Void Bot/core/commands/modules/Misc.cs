using Discord;
using Discord.Commands;
using Discord_Void_Bot.config;
using Discord_Void_Bot.core.data;
using NReco.ImageGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.core.commands.modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

           string msg = $@":information_source:\n";

            msg += "\n**Guest Commands**";
            msg += "\n-agree";

            msg += "\n**User Commands**";
            msg += "\n-mylevel (-xp, -level, -myxp, -profile, -account) -> This shows your profile to yourself and others!";
            msg += "\n-report -> This allows the users to report another user! {<mention>, <reason>}";

            msg += "\n";

            msg += "\n**Administrative Commands**";
            msg += "\n-embed -> This makes a embed out of a message that you give it! {<message>}";
            msg += "\n-kick -> This will kick a member from the community discord! {<mention>, <reason>}";
            msg += "\n-ban -> This will bans a member from the community discord! {<mention>, <howlong>, <reason>}";
            msg += "\n-warn -> This allows the admin to inspect the player! {<reason>}";
            msg += "\n-nuke -> This will remove any message with the given parameters! {<limit>}";
            msg += "\n-inspect -> This allows the admin to inspect the player! {<mention>}";

            msg += "\n";

            msg += "\n**Developer Commands**";
            msg += "\n-setup -> This allows the administrators to configure the bot! {<Conditional|ShouldContinue>, [<Conditional|ShouldCreateRoles>, <Conditional|ShouldCreateNewComersChannel>, <Conditional|ShouldCreateLogsChannel>, <Conditional|ShouldCreateMutedChannel>]}";
            msg += "\n-setlogs -> Sets the channel of where the logs will be held! {<Text Channel Mention>}";
            msg += "\n-setmuted -> Sets the channel of where the muted will be held! {<Text Channel Mention>}";
            msg += "\n-setnewcomers -> Sets the channel of where the new comers will be held! {<Text Channel Mention>}";
            msg += "\n-echo -> This makes the bot speak! {<message>}";


            var embed = new EmbedBuilder();
            embed.WithTitle("Help");
            embed.WithDescription(msg);
            embed.WithColor(new Color(0, 255, 0));

            await dmChannel.SendMessageAsync("", false, embed);

        }

        [Command("welcome")]
        public async Task Welcome()
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            string greating = @"
                :white_check_mark: 
                Welcome {0} to {1}! 
                If you need help, please type `help` either in here, or type `{2}help` or mention me then do `help` by it's self. 
                Thank you and have a great day :smile_cat:!
                ~ {1}'s Staff Team :stars:";
            if (Config.configurations.howToGreatPlayers != null || Config.configurations.howToGreatPlayers != "")
                greating = Config.configurations.howToGreatPlayers;

            greating = String.Format(greating, Context.User.Username, Context.Guild.Name, Config.configurations.cmdPrefix);

            var embed = new EmbedBuilder();
            embed.WithTitle("Void");
            embed.WithDescription(greating);
            embed.WithColor(new Color(0, 255, 0));

            await dmChannel.SendMessageAsync("", false, embed);
        }

        [Command("mylevel")]
        [Alias("level", "xp", "myxp", "profile", "account")]
        public async Task ShowProfile()
        {
            var account = UserAccounts.GetAccount(Context.User);

            double xpBar = 0;
            
            try
            {
                xpBar = ((1.0 + account.XP) / account.NextNeededXP) * 100.0;
            }
            catch { }
            string html = "<!DOCTYPE html>  <html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"> <head>     <meta charset=\"utf-8\" />     <title></title>     <!-- Latest compiled and minified CSS -->     <link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css\">      <!-- jQuery library -->     <script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js\"></script>      <!-- Latest compiled JavaScript -->     <script src=\"https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js\"></script>     <style>         body {             background-color: transparent;         }     </style> </head> <body>     <div class=\"progress\">         <div class=\"progress-bar progress-bar-striped progress-bar-animated\" role=\"progressbar\" style=\"width: " + $"{xpBar}" + "%;\" aria-valuenow=\"" + $"{xpBar}"+"\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>     </div> </body> </html>";


            var converter = new HtmlToImageConverter
            {
                Width = 200,
                Height = 70
            };
            var pngBytes = converter.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Png);
            
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Green);
            embed.WithTitle(Context.User.Username + "'s Profile:");
            embed.AddInlineField("LEVEL", account.Level);
            embed.AddInlineField("XP", account.XP);
            embed.AddInlineField("NEXT LEVEL REQ", account.NextNeededXP);
            await Context.Channel.SendMessageAsync("", embed: embed);
            await Context.Channel.SendFileAsync(new MemoryStream(pngBytes), Context.User.Username + "_profile.png");
        }


        [Command("embed")]
        public async Task Embed([Remainder]string msg)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Posted by " + Context.User.Username);
            embed.WithDescription(msg);
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
