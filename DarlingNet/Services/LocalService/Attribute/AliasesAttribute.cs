using DarlingNet.Services.LocalService.CommandList;
using Discord.Commands;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DarlingNet.Services.LocalService.Attribute
{
    public class AliasesAttribute : AliasAttribute
    {
        public AliasesAttribute([CallerMemberName] string memberName = "") : base(Initiliaze.Load(memberName.ToLowerInvariant()).Usage.Skip(1).ToArray()) { }
    }
}
