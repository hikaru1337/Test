using DarlingNet.Services.LocalService.CommandList;
using Discord.Commands;
using System.Runtime.CompilerServices;

namespace DarlingNet.Services.LocalService.Attribute
{
    public class CommandsAttribute : CommandAttribute
    {
        public CommandsAttribute([CallerMemberName] string memberName = "") : base(Initiliaze.Load(memberName.ToLowerInvariant()).Usage[0]) { }
    }
}
