using System;

namespace DarlingNet.Services.LocalService.SpamCheck
{
    public class DosStructure
    {
        public ulong UsersId { get; set; }
        public ulong GuildsId { get; set; }
        public DateTime Time { get; set; }
    }
}
