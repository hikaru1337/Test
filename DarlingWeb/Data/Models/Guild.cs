﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;

namespace DarlingWeb.Data.Models
{
    public class Guild
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("owner")]
        public bool Owner { get; set; }

        [JsonProperty("permissions")]
        public long Permissions { get; set; }

        [JsonProperty("features")]
        public List<string> Features { get; set; }

        public static List<Guild> ListFromJson(string json) => JsonConvert.DeserializeObject<List<Guild>>(json, UserService.Settings);

    }
}

