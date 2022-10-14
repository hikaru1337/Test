using DarlingNet.Services.LocalService.CommandList;
using Discord.Commands;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace DarlingNet.Services.LocalService.Attribute
{
    public class UsageAttribute : RemarksAttribute
    {
        public UsageAttribute([CallerMemberName] string memberName = "") : base(GetUsage(memberName)) { }

        public static string GetUsage(string memberName)
        {
            var usage = Initiliaze.Load(memberName.ToLowerInvariant()).Usage;
            return JsonConvert.SerializeObject(usage);
        }
    }
}
