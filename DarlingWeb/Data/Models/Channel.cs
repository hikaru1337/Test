using Newtonsoft.Json;
using System.Collections.Generic;

namespace DarlingWeb.Data.Models
{
    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        public ChannelType TypeConvert
        {
            get
            {
               return (ChannelType)Type;
            }
        }

        public static List<Channel> ListFromJson(string json) => JsonConvert.DeserializeObject<List<Channel>>(json, UserService.Settings);

        public enum ChannelType
        {
            GUILD_TEXT,
            DM,
            GUILD_VOICE,
            GROUP_DM,
            GUILD_CATEGORY,
            GUILD_NEWS,
            GUILD_NEWS_THREAD,
            GUILD_PUBLIC_THREAD,
            GUILD_PRIVATE_THREAD,
            GUILD_STAGE_VOICE,
            GUILD_DIRECTORY,
            GUILD_FORUM
        }
    }
}
