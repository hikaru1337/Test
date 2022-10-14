

using static DarlingDb.Enums;

namespace DarlingDb.Models.Pet
{
    public class Items
    {
        public ulong Id { get; set; }
        public string Emoji { get; set; }
        public string Name { get; set; }
        public ulong PetsId { get; set; }
        public Pets Pets { get; set; }
        public uint Price { get; set; }
        public sbyte Value { get; set; }
        public PetItemEnum ItemType { get; set; }
    }
}
