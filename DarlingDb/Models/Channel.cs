using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DarlingDb.Models
{
    public class Channel
    {
        public ulong Id { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public ICollection<Invites> Invites { get; set; }
        public ICollection<Tasks> Tasks { get; set; }
        public ICollection<GiveAways> GiveAways { get; set; }
        public ICollection<EmoteClick> EmoteClick { get; set; }
        public ICollection<ButtonClick> ButtonClick { get; set; }
        public ICollection<Guilds_Logs> Guilds_Logs { get; set; }
        public Guilds_Meeting_Leave Guilds_Meeting_Leave { get; set; }
        public Guilds_Meeting_Welcome Guilds_Meeting_Welcome { get; set; }
        public Guilds_Captcha Guilds_Captcha { get; set; }
        public bool UseCommand { get; set; }
        public bool UseAdminCommand { get; set; }
        public bool UseRPcommand { get; set; }
        public bool GiveXP { get; set; }
        public bool DelUrl { get; set; }
        public bool DelUrlImage { get; set; }
        public bool DelCaps { get; set; }
        public bool Spaming { get; set; }
        public bool SendBadWord { get; set; }
        [NotMapped]
        public List<string> BadWordList
        {
            get
            {
                List<string> List = new();
                if (!string.IsNullOrWhiteSpace(BadWordString))
                    List = BadWordString.Split(',').ToList();
                
                return List;
            }
            set
            {
                BadWordString = string.Join(",", value.ToArray());
            }
        }
        public string BadWordString { get; set; }
        [NotMapped]
        public List<string> csUrlWhiteListList
        {
            get
            {
                List<string> List = new();
                if (!string.IsNullOrWhiteSpace(csUrlWhiteListString))
                    List = csUrlWhiteListString.Split(',').ToList();
                
                return List;
            }
            set
            {
                csUrlWhiteListString = string.Join(",", value.ToArray());
            }
        }
        public string csUrlWhiteListString { get; set; }
        public bool InviteMessage { get; set; }
    }
}
