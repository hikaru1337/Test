namespace DarlingDb.Models
{
    public class Guilds_Meeting_Welcome
    {
        public uint Id { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public string WelcomeMessage { get; set; }
        public string WelcomeDMmessage { get; set; }
        public bool WelcomeDMuser { get; set; }
        public ulong WelcomeChannelId { get; set; }
        public Channel WelcomeChannel { get; set; }
        public ulong? WelcomeRoleId { get; set; }
        public Role WelcomeRole { get; set; }
    }
}
