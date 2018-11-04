using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Void_Bot.core.data
{
    public struct ActionReport
    {
        public string actionerName;
        public ulong actionerID;

        public string reason;

        public string date;

        public int howLong;
    }

    public class UserAccount
    {
        public ulong ID { get; set; }

        public uint Points { get; set; }
        public uint XP { get; set; }


        public uint NextNeededXP
        {
            get
            {
                return (uint)Math.Pow(Level + 1, 2) * 50;
            }
        }

        public uint Level
        {
            get
            {
                return (uint)Math.Sqrt(XP / 50);
            }
        }

        public bool IsMuted { get; set; }

        public List<ActionReport> Warnings = new List<ActionReport>();
        public List<ActionReport> Kicks = new List<ActionReport>();
        public List<ActionReport> Bans = new List<ActionReport>();

        public string FirstJoinDate { get; set; }

        public string LastMessage { get; set; }
        public ulong LastMessageID { get; set; }
        public int CountOfLastMessage { get; set; }
    }
}
