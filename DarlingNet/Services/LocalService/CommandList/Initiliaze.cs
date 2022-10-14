using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DarlingNet.Services.LocalService.CommandList
{
    public class Initiliaze
    {
        private static readonly Dictionary<string, Commands> _commandData;
        static Initiliaze()
        {
            _commandData = JsonConvert.DeserializeObject<Dictionary<string, Commands>>(File.ReadAllText("CommandsList.json"));
        }
        public static Commands Load(string key)
        {
            _commandData.TryGetValue(key, out var toReturn);

            if (toReturn == null)
                toReturn = new Commands
                {
                    Category = "",
                    MinDesc = key,
                    Desc = key,
                    Usage = new[] { key, key }
                };

            else if (string.IsNullOrWhiteSpace(toReturn.MinDesc))
                toReturn.MinDesc = toReturn.Desc;

            if (toReturn.Usage[0]?.Length == 0 && toReturn.Usage[1]?.Length == 0)
                toReturn.Usage = new[] { key, key };

            if (!ListCommand.Any(x=>x.Usage[1] == toReturn.Usage[1]))
                ListCommand.Add(toReturn);
            return toReturn;
        }

        public static List<Commands> ListCommand = new();

        public class Commands
        {
            public string Category { get; set; }
            public string MinDesc { get; set; }
            public string Desc { get; set; }
            public string[] Usage { get; set; }
        }
    }
}
