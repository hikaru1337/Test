using DarlingNet.Services.LocalService.CommandList;
using Discord.Commands;
using System.Runtime.CompilerServices;

namespace DarlingNet.Services.LocalService.Attribute
{
    class DescriptionsAttribute : SummaryAttribute
    {
        public DescriptionsAttribute([CallerMemberName] string memberName = "") : base(Initiliaze.Load(memberName).Desc) { }
    }
}
