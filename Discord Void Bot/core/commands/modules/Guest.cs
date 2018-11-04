using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.core.commands.modules
{
    public class Guest : ModuleBase<SocketCommandContext>
    {
        [Command("agree")]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task Agree()
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Member");
            Console.WriteLine(role.Name);
            await (Context.User as IGuildUser).AddRoleAsync(role);
        }
    }
}
