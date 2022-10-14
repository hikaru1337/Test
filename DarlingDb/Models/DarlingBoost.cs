using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarlingDb.Models
{
    public class DarlingBoost
    {
        public uint Id { get; set; }
        public ulong UsersId { get; set; }
        public Users Users { get; set; }
        public ushort Streak { get; set; }
        public DateTime Ends { get; set; }

        [NotMapped]
        public bool Active
        {
            get
            {
                if (Ends > DateTime.Now)
                    return true;

                return false;
            }
        }
    }
}
