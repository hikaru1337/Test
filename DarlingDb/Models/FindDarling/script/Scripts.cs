//using System.Collections.Generic;

//namespace DarlingDb.Models.FindDarling.script
//{

//    public class Constructor
//    {
//        public State ToStation { get; set; }
//        public string TextMenu { get; set; }
//        public string TextButton { get; set; }
//        public Type TypeThis { get; set; }
//        public enum Type
//        {
//            Button,
//            Auto,
//            GetMessage
//        }
//    }


//    class Scripts
//    {
//        public static Dictionary<State, Constructor> GetItem(State s) => scripts[s];

//        private static Constructor Create(string TextButton, string TextMenu = "-", Constructor.Type type = Constructor.Type.Button)
//        {
//            if (TextMenu == "-")
//                TextMenu = TextButton;
//            return new Constructor { TextButton = TextButton, TextMenu = TextMenu, ToStation = State.none, TypeThis = type };
//        }


//        private static readonly Dictionary<State, Dictionary<State, Constructor>> scripts = new()
//        {
//            [State.Menu] = new()
//            {
//                  { State.CreateAccount, Create("Создать аккаунт") },
//                  { State.MyAnkete, Create("Ваша анкета") },
//                  { State.WatchAnkete, Create("Смотреть анкеты") },
//                  { State.WatchReports, Create("Смотреть репорты") },
//                  { State.OffAnkete, Create("Отключить анкету") },
//                  { State.Rules, Create("Правила") },
//             },

//            [State.CreateAccount] = new()
//            {
//                { State.Menu, Create("Меню") },
//                { State.CreateName, Create("-", "Введите имя:", Constructor.Type.GetMessage) },
//            },

//            [State.CreateName] = new()
//            {
//                { State.Menu, Create("Меню") },
//                { State.CreateDescription, Create("-", "Введите описание:", Constructor.Type.GetMessage) },
//            },

//            [State.CreateDescription] = new()
//            {
//                { State.Menu, Create("Меню") },
//                { State.CreateYear, Create("-", "Введите возраст:",   Constructor.Type.GetMessage) },
//            },

//            [State.CreateYear] = new()
//            {
//                { State.Menu, Create("Меню") },
//                { State.CreatePhoto, Create("-", "Отправьте фото:",  Constructor.Type.GetMessage) },
//            },

//            [State.CreatePhoto] = new()
//            {
//                { State.Menu, Create("Меню") },
//                { State.CreateHobby, Create("-", "Введите хобби",  Constructor.Type.GetMessage) },
//            },

//            [State.CreateHobby] = new()
//            {
//                { State.Menu, Create("Меню") },
//                { State.CreateSex, Create("-", "Введите пол:",   Constructor.Type.GetMessage) },
//            },

//            [State.CreateSex] = new()
//            {
//                { State.Menu, Create("Меню") },
//                { State.CreateSexPartner, Create("-", "Введите пол собеседника:",   Constructor.Type.GetMessage) },
//            },

//            [State.CreateSexPartner] = new()
//            {
//                { State.MyAnkete, Create("","-",  Constructor.Type.Auto) },
//            },

//            [State.OffetAnkete] = new()
//            {
//                  { State.OffAnkete, Create("Отключить анкету?","Отключить!") },
//                  { State.Menu, Create("Отмена") },
//             },

//            [State.OffAnkete] = new()
//            {
//                  { State.OnAnkete, Create("Включить анкету?", "Включить!") },
//             },

//            [State.OnAnkete] = new()
//            {
//                  { State.Menu, Create("", "-",  Constructor.Type.Auto) },
//             },

//            [State.Rules] = new()
//            {
//                  { State.RulesAccept, Create("Принять правила!", "Принять!") },
//                  { State.Menu, Create("Отмена") },
//             },

//            [State.RulesAccept] = new()
//            {
//                  { State.Menu, Create("", "-",  Constructor.Type.Auto) },
//             },

//            [State.WatchReports] = new()
//            {
//                  { State.ReportAnkete, Create("Репорт") },
//                  { State.NotReportedAnkete, Create("Не репорт") },
//                  { State.Menu, Create("Меню") },
//             },

//            [State.ReportAnkete] = new()
//            {
//                  { State.WatchReports, Create("", "-",  Constructor.Type.Auto) },
//             },

//            [State.NotReportedAnkete] = new()
//            {
//                  { State.WatchReports, Create("", "-",  Constructor.Type.Auto) },
//             },

//            [State.LikeAnketeLike] = new()
//            {
//                { State.WatchLiked, Create("", "-", Constructor.Type.Auto) },
//            },

//            [State.DisLikeAnketeLike] = new()
//            {
//                { State.WatchLiked, Create("", "-",  Constructor.Type.Auto) },
//            },


//            [State.ReportAnkete] = new()
//            {
//                { State.WatchReports, Create("", "-",  Constructor.Type.Auto) },
//            },

//            [State.NotReportedAnkete] = new()
//            {
//                { State.WatchReports, Create("", "-",  Constructor.Type.Auto) },
//            },


//            [State.WatchAnkete] = new()
//            {
//                  { State.LikeAnkete, Create("Лайк") },
//                  { State.DisLikeAnkete,  Create("Дизлайк") },
//                  { State.ReportAnkete, Create("Репорт") },
//                  { State.Menu, Create("Меню") },
//             },

//            [State.LikeAnkete] = new()
//            {
//                  { State.WatchAnkete, Create("", "-",  Constructor.Type.Auto) },
//             },

//            [State.DisLikeAnkete] = new()
//            {
//                  { State.WatchAnkete, Create("", "-",  Constructor.Type.Auto) },
//             },

//            [State.ReportAnkete] = new()
//            {
//                  { State.WatchAnkete, Create("", "-",  Constructor.Type.Auto) },
//             },





//            [State.MyAnkete] = new()
//            {
//                  { State.EditName, Create("Изменить имя") },
//                  { State.EditDescription, Create("Изменить описание") },
//                  { State.EditYear, Create("Изменить возраст") },
//                  { State.EditHobby, Create("Изменить увлечения") },
//                  { State.EditPhoto, Create("Изменить фото") },
//                  { State.Menu, Create("Меню") },
//             },

//            [State.EditName] = new()
//            {
//                 { State.Menu, Create("", "-",  Constructor.Type.GetMessage) },
//             },

//            [State.EditDescription] = new()
//            {
//                { State.Menu, Create("", "-",  Constructor.Type.Auto) },
//            },

//            [State.EditYear] = new()
//            {
//                { State.Menu, Create("", "-",  Constructor.Type.Auto) },
//            },

//            [State.EditPhoto] = new()
//            {
//                { State.Menu, Create("", "-",  Constructor.Type.Auto) },
//            },

//            [State.EditHobby] = new ()
//             {
//                   { State.Menu, Create("", "-",  Constructor.Type.Auto) },
//             },
//        };
//    }
//}