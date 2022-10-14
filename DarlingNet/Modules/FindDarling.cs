//using DarlingDb;
//using DarlingDb.Models.FindDarling;
//using DarlingDb.Models.FindDarling.script;
//using DarlingNet.Services.LocalService.GetOrCreate;
//using DarlingNet.Services.LocalService.VerifiedAction;
//using Discord;
//using Discord.Commands;
//using Discord.WebSocket;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace DarlingNet.Modules
//{
//    [RequireOwner]
//    public class FindDarling : ModuleBase<ShardedCommandContext>
//    {
//        //[Aliases, Commands, Usage, Descriptions]
//        [Command("start")]
//        public async Task start()
//        {
//            await StationGenerate(Context.User);
//        }

//        [Command("reporter")]
//        public async Task reporter(ulong UserId = 0)
//        {
//            using (DBcontext _db = new())
//            {
//                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithDescription("");
//                if (UserId == 0)
//                {
//                    emb.WithAuthor("Пользователи с правами репортера!");
//                    foreach (var User in _db.Darling.Where(x => x.ReportPermission))
//                    {
//                        emb.Description += $"{User.Id}";
//                    }
//                }
//                else
//                {
//                    emb.WithAuthor("Права репортера!");
//                    var Darling = _db.Darling.FirstOrDefault(x => x.UsersId == UserId);
//                    if (Darling == null)
//                        emb.WithDescription($"Пользователь с таким Id `{UserId}` не найден!");
//                    else
//                    {
//                        if (Darling.ReportPermission)
//                            emb.WithDescription("Права были успешно сняты!");
//                        else
//                            emb.WithDescription("Права были успешно выданы!");
//                        Darling.ReportPermission = !Darling.ReportPermission;
//                        _db.Darling.Update(Darling);
//                        await _db.SaveChangesAsync();
//                    }
//                }
//                await Context.User.UserMessage("", emb);
//            }
//        }


//        public static async Task StationGenerate(SocketUser User)
//        {
//            using (DBcontext _db = new())
//            {
//                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithDescription("");
//                var Darling = _db.Darling.Include(x => x.Hobbies).Include(x => x.DarlingLast).Include(x => x.ReportLast).FirstOrDefault(x => x.UsersId == User.Id);
//                if (Darling == null)
//                {
//                    await _db.Users.GetOrCreate(User.Id);
//                    Darling = new Darling { UsersId = User.Id, State = State.Menu };
//                    _db.Darling.Add(Darling);
//                    await _db.SaveChangesAsync();
//                }
//                var currentState = Scripts.GetItem(Darling.State);
//                var Comp = new ComponentBuilder();
//                emb.WithAuthor($"{Darling.State}");
//                foreach (var item in currentState)
//                {
//                    switch (item.Value.TypeThis)
//                    {
//                        case Constructor.Type.Button:
//                            bool notadd = false;

//                            switch (Darling.State)
//                            {
//                                case State.Menu:
//                                    if (Darling.Year == 0 || string.IsNullOrWhiteSpace(Darling.Name) || string.IsNullOrWhiteSpace(Darling.Description))
//                                    {
//                                        if (item.Key != State.CreateAccount)
//                                            notadd = true;
//                                    }
//                                    if (!Darling.ReportPermission && item.Key == State.WatchReports)
//                                        notadd = true;
//                                    break;
//                                case State.MyAnkete:
//                                    break;
//                                case State.CreatePhoto:
//                                    foreach (var Hobbie in _db.Darling_Hobbies)
//                                        item.Value.TextMenu += $"\n- {Hobbie.Name}";
//                                    break;
//                                case State.OffAnkete:
//                                    Darling.Visible = false;
//                                    await _db.SaveChangesAsync();
//                                    break;
//                            }

//                            if (!notadd)
//                            {
//                                emb.Description += $"{item.Value.TextMenu}\n";
//                                Comp.WithButton($"{item.Value.TextButton}", $"DarlingFind_{item.Key}");
//                            }


//                            break;
//                        case Constructor.Type.Auto:
//                            switch (Darling.State)
//                            {
//                                case State.OnAnkete:
//                                    Darling.Visible = true;
//                                    break;
//                                case State.LikeAnkete:
//                                    if (Darling.DarlingLast != null)
//                                        _db.Darling_LikeDisLike.Add(new Darling_LikeDisLike { FirstDarlingId = Darling.Id, SecondDarlingId = Convert.ToUInt64(Darling.DarlingLastId), Time = System.DateTime.Now, ThisState = Darling_LikeDisLike.State.Like });

//                                    foreach (var This in _db.Darling.Where(x => x.AnketeWork && x.SexSearch == Darling.Sex && x.Sex == Darling.SexSearch))
//                                    {
//                                        if (!_db.Darling_LikeDisLike.Any(x => (x.FirstDarlingId == Darling.Id && x.SecondDarlingId == This.Id) || (x.FirstDarlingId == This.Id && x.SecondDarlingId == Darling.Id)))
//                                        {
//                                            Darling.DarlingLastId = This.Id;
//                                            break;
//                                        }
//                                    }
//                                    break;
//                                case State.DisLikeAnkete:
//                                    if (Darling.DarlingLast != null)
//                                        _db.Darling_LikeDisLike.Add(new Darling_LikeDisLike { FirstDarlingId = Darling.Id, SecondDarlingId = Convert.ToUInt64(Darling.DarlingLastId), Time = System.DateTime.Now, ThisState = Darling_LikeDisLike.State.DisLike });
//                                    foreach (var This in _db.Darling.Where(x => x.AnketeWork && x.SexSearch == Darling.Sex && x.Sex == Darling.SexSearch))
//                                    {
//                                        if (!_db.Darling_LikeDisLike.Any(x => (x.FirstDarlingId == Darling.Id && x.SecondDarlingId == This.Id) || (x.FirstDarlingId == This.Id && x.SecondDarlingId == Darling.Id)))
//                                        {
//                                            Darling.DarlingLastId = This.Id;
//                                            break;
//                                        }
//                                    }
//                                    break;
//                                case State.ReportAnkete:
//                                    if (Darling.DarlingLast != null)
//                                        _db.Darling_Reports.Add(new Darling_Reports { Id = Convert.ToUInt64(Darling.DarlingLastId), Description = Darling.DarlingLast.Description, Name = Darling.DarlingLast.Name, ImageUrl = Darling.DarlingLast.ImageUrl });
//                                    foreach (var This in _db.Darling.Where(x => x.AnketeWork && x.SexSearch == Darling.Sex && x.Sex == Darling.SexSearch))
//                                    {
//                                        if (!_db.Darling_LikeDisLike.Any(x => (x.FirstDarlingId == Darling.Id && x.SecondDarlingId == This.Id) || (x.FirstDarlingId == This.Id && x.SecondDarlingId == Darling.Id)))
//                                        {
//                                            Darling.DarlingLastId = This.Id;
//                                            break;
//                                        }
//                                    }
//                                    break;
//                                case State.ReportedAnkete:
//                                    Darling.ReportLast.ReportTrue = true;
//                                    _db.Darling_Reports.Update(Darling.ReportLast);
//                                    break;
//                                case State.NotReportedAnkete:
//                                    Darling.ReportLast.ReportTrue = false;
//                                    _db.Darling_Reports.Update(Darling.ReportLast);
//                                    break;
//                                case State.RulesAccept:
//                                    Darling.AcceptRules = true;
//                                    break;
//                                case State.EditName:
//                                case State.CreateName:
//                                    Darling.Name = Darling.LastMessage;
//                                    break;
//                                case State.EditDescription:
//                                case State.CreateDescription:
//                                    Darling.Name = Darling.LastMessage;
//                                    break;
//                                case State.EditYear:
//                                case State.CreateYear:
//                                    Darling.Name = Darling.LastMessage;
//                                    break;
//                                case State.EditPhoto:
//                                case State.CreatePhoto:
//                                    Darling.ImageUrl = Darling.LastMessage;
//                                    break;
//                                case State.EditHobby:
//                                case State.CreateHobby:
//                                    try
//                                    {
//                                        var list = Darling.LastMessage.Split(',');
//                                        foreach (string word in list)
//                                        {
//                                            string wword = word.ToLower();
//                                            var Hobbies = _db.Darling_Hobbies.FirstOrDefault(x => x.Name.ToLower() == wword);
//                                            if (Hobbies != null)
//                                            {
//                                                Darling.Hobbies.Add(Hobbies);
//                                            }
//                                        }
//                                    }
//                                    catch (Exception)
//                                    {
//                                        emb.Description = "Ошибка в введении хобби. Напишите из через запятую: Dota2,csgo,minecraft...";
//                                    }
//                                    break;
//                            }
//                            Darling.State = item.Key;// item.Value.ToStation;
//                            _db.Darling.Update(Darling);
//                            await _db.SaveChangesAsync();
//                            await StationGenerate(User);
//                            return;
//                        case Constructor.Type.GetMessage:
//                            Darling.State = item.Key;
//                            _db.Darling.Update(Darling);
//                            await _db.SaveChangesAsync();
//                            emb.Description += $"{item.Value.TextMenu}\n";
//                            break;
//                    }
//                }
//                await User.UserMessage("", emb, Comp.Build());
//            }
//        }



//        private void Menu(bool HaveAccount, State State) // Меню 2 состояния, есть акк, нету акка
//        {
//            using (DBcontext _db = new())
//            {
//                if (HaveAccount)
//                {

//                }
//                else
//                {

//                }
//            }
//        }

//        private void YourAnkete() // Ваша анкета
//        {

//        }

//        private void PartnerAnkete() // Анкета партнера при рассмотрении заявок
//        {

//        }

//        private void PartnerAnketeLike() // Партнеры которые вас лайкнули
//        {

//        }

//        private void PartnerAnketeReport() // Партнеры которые получили репорты
//        {

//        }

//        private void OnAndOff() // Включить или отключить анкету
//        {

//        }

//        private void Rules() // Правила
//        {

//        }

//        private void GetName(ulong UserId, string Name) // Имя пользователя, 2 состояния, при первом заполнеии, и при изменении
//        {

//        }

//        private void GetYear(ulong UserId, sbyte Year) // Возраст пользователя, 2 состояния, при первом заполнеии, и при изменении
//        {

//        }

//        private void GetPhoto(ulong UserId, string Photo) // Фото пользователя, 2 состояния, при первом заполнеии, и при изменении
//        {

//        }

//        private void GetDescription(ulong UserId, string Description) // Описание пользователя, 2 состояния, при первом заполнеии, и при изменении
//        {

//        }

//        private void GetHobbys(ulong UserId, string Hobbys) // Хобби пользователя, 2 состояния, при первом заполнеии, и при изменении
//        {

//        }

//        private void GetSex(ulong UserId, Darling.Gender Gender, bool Your = true) // Пол пользователя или партнера, при первом заполнеии
//        {

//        }
//    }
//}
