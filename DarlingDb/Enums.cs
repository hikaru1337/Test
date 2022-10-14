namespace DarlingDb
{
    public class Enums
    {
        public enum ButtonActionEnum : byte 
        {
            Marryed_Wait,
            Marryed_Yes,
            Marryed_No,
            LeftRight_Wait,
            LeftRight_Left,
            LeftRight_Right,
            Number_Wait,
            Number_1,
            Number_2,
            Number_3,
            Number_4,
            Number_5
        } // NotMessages 

        public enum KazinoChipsEnum : byte
        {
            allzero,
            allblack,
            allred,
            zero,
            black,
            red
        }

        public enum VoiceAuditActionEnum : byte 
        {
            AdminMute,
            AdminUnMute,
            AdminDeafened,
            AdminUnDeafened,
            Defect
        } // NotMessages

        public enum StatusTicketEnum : byte
        {
            Отправлен,
            Рассмотрение,
            Исправлен,
            Отклонен,
            Удалена,
            none,
        }

        public enum ViolationSystemEnum : byte
        {
            none,
            off,
            WarnSystem,
            ReportSystem
        }

        public enum ChannelsTypeEnum : byte 
        {
            Ban,
            UnBan,
            Kick,
            Left,
            Join,
            MessageEdit,
            MessageDelete,
            VoiceAction,
            BirthDay
        } // NotMessages 

        public enum ReportTypeEnum : byte
        {
            TimeBan,
            Mute,
            TimeOut,
            Kick,
            Ban
        }

        public enum PetItemEnum : byte
        {
            Еда,
            МедПомощь
        }

        public enum PetTypesEnum : byte
        {
            none,
            Котик,
            Собачка,
            Попугай,
            Крыска,
            Хомячок,
            Кролик
        }

        public enum RoleTypeEnum : byte 
        {
            Buy,
            Give,
            Level,
            Time
        } // NotMessages 
    }
}
