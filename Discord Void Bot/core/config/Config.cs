using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.config
{
    class Config
    {

        private const string configFolder = "resources";
        private const string configFile = "config.json";

        public static BotConfiguration configurations;

        static Config()
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                // create it...
                configurations = new BotConfiguration();
                Save();
            }
            else
            {
                Load();
            }
        }

        public static void Save()
        {
            File.Delete(configFolder + "/" + configFile);

            Console.WriteLine("Saved Configurations");
            string json = JsonConvert.SerializeObject(configurations, Formatting.Indented);
            File.WriteAllText(configFolder + "/" + configFile, json);
        }

        public static void Load()
        {
            Console.WriteLine("Loaded Configurations");
            string json = File.ReadAllText(configFolder + "/" + configFile);
            configurations = JsonConvert.DeserializeObject<BotConfiguration>(json);

            if (configurations == null)
            {
                Console.WriteLine("[ Critical Failure ] :: Configurations couldn't been loaded!");
            }
        }
    }

    public class BotConfiguration
    {
        // Developer Config
        public string token = "NDc1NDk2MzkwMTMxMzE4Nzk1.DkgDgg.kxOXSbfR9d9gkxEXmWTU6ALPYZ4";

        // User Configurations
        public string cmdPrefix = "-";
        public string howToGreatPlayers = @"
                :white_check_mark: 
                Welcome {0} to {1}! 
                If you need help, please type `help` either in here, or type `{2}help` or mention me then do `help` by it's self. 
                Thank you and have a great day :smile_cat:!
                ~ {1}'s Staff Team :stars:";

        public string communityWebsite = "www.example.com";

        public float deletionDelay = 5;

        // Warn System
        public bool banWarnThreshHold = true;
        public int banThreashHold = 3;

        public bool muteWarnThreashHold = true;
        public int muteThreashHold = 2;

        public ulong welcomeChannelID;
        public ulong logsChannelID;
        public ulong mutedChannelID;

        public ulong MemberID;
        public ulong TrustedID;
        public ulong MutedID;
    }

    public class GuildConfiguration
    {
        public string guild;
    }
}
