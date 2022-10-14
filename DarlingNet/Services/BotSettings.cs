using System.Collections.Generic;

namespace DarlingNet.Services
{
    public class BotSettings
    {
        public static bool BotReady = false;
        public static int _totalShards = 2;
        public static int ShardReadyCount = 0;
        public static string Version = "0.3 OPEN-TEST";

        public static string config_file = "_config.yml";
        public static string ConnectionString = @"Data Source = DarlingNet.db";
        public static string Prefix = "test.";

        public static string bannerBoturl = "https://cdn.discordapp.com/attachments/642712334145421321/733726055382122566/-1_1.jpg";
        public static string EnableDMmessageURL = "https://media.discordapp.net/attachments/642712334145421321/738149876549812375/assets2F-MBTPU7ahRWnPag_7AsM2F-MDRDtMippLlKyFKIQtV2F-MDRFGCyncfIm-uyHmhs2Fimage.png";
        public static string PayURL = "https://bill.discord-bot.net/botpay/get?token=5e43cb4e72a78";
        public static string PayUserURL = "https://bill.discord-bot.net/botpay/user?owner_id=551373471536513024&discord_id={0}&payment_method=qiwi&amount={1}";

        public static ulong hikaruid = 551373471536513024;
        public static ulong botid = 663381953181122570;
        public static ulong ChannelScreenshots = 947620587277254666;
        public static ulong ChannelError = 947620782954119268;

        public static string EmoteOn = "<:on:947621024453787668>";
        public static string EmoteOff = "<:off:947621024386646036>";
        public static string EmoteWhat = "<:What:947621024579596328>";
        public static string EmoteBoostNot = "<:boostNoLastDay:947621157090259015>";
        public static string EmoteBoostUp = "<:boostUp:947621375118561310>";
        public static string EmoteBoostNo = "<:boostNo:947621156872146995>";
        public static string EmoteBoost = "<:boost:947621217379172422>";
        public static string EmoteBoostLastDay = "<:BoostNoLastDay:772846181268586517>";

        public static List<string> CommandsForBot = new()
        {
            "es",
            "embedsay",
            "userclear",
            "uclr"
        };
        public static List<string> CommandNotInvise = new()
        {
            "modules",
            "m",
            "commands",
            "c",
            "info",
            "i",
            "use",
            "u",
            "commandinvise",
            "ci",
            "application",
            "feedback",
            "fb",
            "myfeedback",
            "mfb",
            "boost",
            "invitebot"
        };

        public static List<string> AdminCommands = new()
        {
            "ban",
            "unban",
            "userclear",
            "clear",
            "kick",
            "mute",
            "unmute",
            "timeout",
            "embedsay",
            "say",
        };

        public static List<string> ViolationCommands = new()
        {
            "warn",
            "report",
            "unwarn",
            "unreport",
            "addreportrules",
            "addreportpunishes",
            "delreportrules",
            "delreportpunishes",
            "addwarn",
            "delwarn",
            "warns",
            "rules",
            "violationsystem"
        };

        public static List<string> RoleCommands = new()
        {
            "levelrole",
            "levelrole.add",
            "levelrole.del",
            "buyrole",
            "buyrole.add",
            "buyrole.del",
            "timerole",
            "timerole.add",
            "timerole.del",
        };

        public static List<string> DetectCommands = new()
        {
            "logsettings",
            "messagesettings",
            "channelsettings",
            "channelsettings.badword",
            "channelsettings.urlwhitelist",
        };

        public static List<string> UserCommands = new()
        {
            "voicepoint",
            "zcoin",
            "daily",
            "transfer",
            "kazino",
            "marry",
            "divorce",
            "usertop",
            "birthdate"
        };

        public static List<string> OwnerCommand = new()
        {
            "blockuser",
            "feedbugs",
        };


        public static string WelcomeText = "⚡️ Бот по стандарту использует префикс: **{0}**\n" +
                                            "• {0}m - список модулей\n" +
                                            "• {0}c [module] - список команд модуля\n" +
                                            "• {0}i [command] - информация о команде\n\n" +
                                            "• {0}application - настройка модулей бота\n\n" +
                                            "🔨 Бот находится в режиме тестирования!\n" +
                                            "🔨 Нашли баг? Пишите - **{0}feedback [описание бага]**\n\n" +
                                            "👑 Инструкция бота - https://docs.darlingbot.ru/ \n" +
                                            "🎁 Добавить бота на сервер - [КЛИК](https://discord.com/oauth2/authorize?client_id={1}&scope=bot&permissions=8)\n\n" +
                                            "Все обновления бота будут выходить тут - [КЛИК](https://docs.darlingbot.ru/obnovleniya)";
    }
}
