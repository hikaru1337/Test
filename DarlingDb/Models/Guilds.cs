using DarlingDb.Models.ReportSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static DarlingDb.Enums;

namespace DarlingDb.Models
{
    public class Guilds
    {
        public ulong Id { get; set; }
        public ICollection<Channel> Channels { get; set; }
        public ICollection<Role> Role { get; set; }
        public ICollection<Captcha> Captcha { get; set; }
        public ICollection<Guilds_Warns> Guilds_Warns { get; set; }
        public ICollection<Users_Guild> Users_Guild { get; set; }
        public ICollection<Reports> Reports { get; set; }
        public Guilds_Captcha Guilds_Captcha { get; set; }
        public Guilds_Raid Guilds_Raid { get; set; }
        public Guilds_Meeting_Leave Guilds_Meeting_Leave { get; set; }
        public Guilds_Meeting_Welcome Guilds_Meeting_Welcome { get; set; }
        public bool Leaved { get; set; }
        public string Prefix { get; set; }
        public ulong PrivateId { get; set; }

        [NotMapped]
        public List<string> CommandInviseList
        {
            get
            {
                List<string> NewList = new();
                if (!string.IsNullOrWhiteSpace(CommandInviseString))
                {
                    NewList = CommandInviseString.Split(',').ToList();
                }
                return NewList;
            }
            set
            {
                CommandInviseString = string.Join(",", value);
            }
        }
        public string CommandInviseString { get; set; }


        [NotMapped]
        public List<ulong> VoiceAndCategoryChannelList
        {
            get
            {
                List<ulong> NewList = new();
                if (!string.IsNullOrWhiteSpace(VoiceAndCategoryChannelString))
                {
                    NewList = Array.ConvertAll(VoiceAndCategoryChannelString.Split(','), ulong.Parse).ToList();
                }
                return NewList;
            }
            set
            {
                VoiceAndCategoryChannelString = string.Join(",", value);
            }
        }
        public string VoiceAndCategoryChannelString { get; set; }

        public ViolationSystemEnum VS { get; set; }
    }
}
