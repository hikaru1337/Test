using Newtonsoft.Json;
using System.Collections.Generic;

namespace DarlingWeb.Data.Models
{
    public class DiscordUser
    {
        public ulong UserId { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; } = null;
        public bool? Verified { get; set; } = null;
        public static List<DiscordUser> ListFromJson(string json) => JsonConvert.DeserializeObject<List<DiscordUser>>(json, UserService.Settings);
    }
}
