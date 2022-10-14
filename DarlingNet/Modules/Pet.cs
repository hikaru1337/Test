using DarlingNet.Services.LocalService.Attribute;
using Discord.Commands;
using System.Threading.Tasks;
using static DarlingNet.Services.LocalService.Attribute.CommandLocksAttribute;
using DarlingDb;
using DarlingDb.Models;
using static DarlingDb.Models.Pets;
using System.Linq;
using Discord;
using System;
using DarlingNet.Services.LocalService.GetOrCreate;
using Discord.WebSocket;
using DarlingDb.Models.Pet;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DarlingNet.Services.LocalService;
using static DarlingDb.Enums;

namespace DarlingNet.Modules
{
    [Pet]
    [Summary("Игра-тамагочи")]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    [RequireOwner]
    public class Pet : ModuleBase<ShardedCommandContext>
    {
        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task pets(PetTypesEnum Pet = PetTypesEnum.none)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94);
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                if (Pet == PetTypesEnum.none)
                {
                    emb.WithAuthor("Выбор питомца");
                    emb.AddField("Котик 😻", "Будет мурчать на ушко\nБез чулков, но тоже ничего...", true);
                    emb.AddField("Собачка 🐶", "Станет вашим верным другом, или подругой.", true);
                    emb.AddField("Крыска 🐭", "Хвостатый друг, лучше твоих подруг.", true);
                    emb.AddField("Попугай 🦜", "Часовые разговоры с подругой?\nПодержите мой клюв.", true);
                    emb.AddField("Хомячок 🐹", "Это не жир, это моя любовь!", true);
                    emb.AddField("Кролик 🐰", "Я не твой парень\nно даже я буду медленее...", true);
                    emb.WithFooter($"Выбрать питомца - {Prefix}pets [Тип питомца]");
                }
                else
                {
                    emb.WithAuthor("Выбор питомца");
                    var CreatePet = new Pets { Name = Pet.ToString(), PetType = Pet, DateOfBirth = DateTime.Now, UserId = Context.User.Id, LastEat = DateTime.Now, LastSleep = DateTime.Now, LastMood = DateTime.Now };
                    byte MaxStartValue = 127;
                    CreatePet.EAT = MaxStartValue;
                    CreatePet.MOOD = MaxStartValue;
                    CreatePet.SLEEP = MaxStartValue;
                    emb.WithDescription($"У вас появился новый любимец `{CreatePet.Name}`").WithFooter($"Назовите своего питомца - {Prefix}petname [имя]");
                    _db.Pets.Add(CreatePet);
                    await _db.SaveChangesAsync();
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task petname([Remainder] string Name)
        {
            using (db _db = new ())
            {
                var GetPet = _db.Pets.FirstOrDefault(x => x.UserId == Context.User.Id);
                GetPet.Name = Name;
                _db.Pets.Update(GetPet);
                await _db.SaveChangesAsync();
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Бирка для питомца");
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                emb.WithDescription($"Вы успешно дали имя своему питомцу.\n\nВстречайте: 🎉 **{Name}** 🎉").WithFooter($"Команды - {Prefix}c pet");
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task petinfo(SocketGuildUser user = null)
        {
            using (db _db = new ())
            {
                if (user == null)
                    user = Context.User as SocketGuildUser;
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Питомец ");
                var GetPet = _db.Pets.Include(x => x.Items).FirstOrDefault(x => x.UserId == user.Id);

                if (GetPet == null || GetPet.Die)
                {
                    if (GetPet.Die && GetPet != null)
                        PetService.PetDie(GetPet, user);
                    emb.Author.Name += "отсустствует!";
                    emb.WithDescription($"У {(user != Context.User ? "данного пользователя" : "вас")} еще нету своего любимца!");
                }
                else
                {
                    emb.Author.Name += GetPet.Name;
                    string petname = GetPet.Name;
                    if (GetPet.Name == GetPet.PetType.ToString())
                    {
                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        petname = $"{Prefix}petname [Имя питомца]";
                    }

                    var count = GetPet.Level * 80 * GetPet.Level;
                    var countNext = (GetPet.Level + 1) * 80 * (GetPet.Level + 1);

                    emb.WithDescription($"Статус: {GetPet.Status}\n\n" +
                                        $"Кличка: {petname}\n" +
                                        $"Здоровье: {GetPet.HP}/{GetPet.MaxHaracter}\n" +
                                        $"Настроение: {GetPet.MOOD}/{GetPet.MaxHaracter}\n" +
                                        $"Сон: {GetPet.SLEEP}/{GetPet.MaxHaracter}\n" +
                                        $"Сытость: {GetPet.EAT}/{GetPet.MaxHaracter}\n" +
                                        $"Уровень: [{GetPet.Level}] [{GetPet.XP - count}/{countNext - count}]\n" +
                                        $"Тип: {GetPet.PetType}");


                    if (GetPet.Items.Count != 0)
                    {
                        string item1 = string.Empty;
                        string item2 = string.Empty;

                        var ListItem = GetPet.Items.OrderBy(x => x.Emoji).ToList();
                        var result = new Dictionary<Items, int>();
                        for (int i = 0; i < ListItem.Count; i++)
                        {
                            if (!result.Keys.Any(x => x.Emoji == ListItem[i].Emoji))
                                result.Add(ListItem[i], ListItem.Count(x => x.Emoji == ListItem[i].Emoji));
                        }

                        foreach (var item in result)
                        {
                            var Thisitem = item.Key;
                            if (Thisitem.ItemType == PetItemEnum.МедПомощь)
                                item1 += $"{Thisitem.Emoji}{Thisitem.Name} [{item.Value}]\n";
                            else
                                item2 += $"{Thisitem.Emoji}{Thisitem.Name} [{item.Value}]\n";
                        }
                        emb.AddField("Мед. помощь", item1?.Length == 0 ? "пусто" : item1, true);
                        emb.AddField("Еда", item2?.Length == 0 ? "пусто" : item2, true);
                    }

                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task peteat(SocketGuildUser user = null)
        {
            using (db _db = new ())
            {
                if (user == null)
                    user = Context.User as SocketGuildUser;
                var GetPet = _db.Pets.Include(x => x.Items).FirstOrDefault(x => x.UserId == user.Id);
                var ThisAction = Action.Покормить;
                var PetScanning = await PetScan(ThisAction, GetPet, user);
                var emb = PetScanning.Item1;
                await Context.Channel.SendMessageAsync("", false, emb.Build());

                if (PetScanning.Item2)
                    await RandomMessage(emb, user, GetPet.Name, ThisAction);
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task pethelp(SocketGuildUser user = null)
        {
            using (db _db = new ())
            {
                if (user == null)
                    user = Context.User as SocketGuildUser;

                var GetPet = _db.Pets.Include(x => x.Items).FirstOrDefault(x => x.UserId == user.Id);
                var ThisAction = Action.Вылечить;
                var PetScanning = await PetScan(ThisAction, GetPet, user);
                var emb = PetScanning.Item1;
                await Context.Channel.SendMessageAsync("", false, emb.Build());

                if (PetScanning.Item2)
                    await RandomMessage(emb, user, GetPet.Name, ThisAction);
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task petmood(SocketGuildUser user = null)
        {
            using (db _db = new ())
            {
                if (user == null)
                    user = Context.User as SocketGuildUser;
                var GetPet = _db.Pets.FirstOrDefault(x => x.UserId == user.Id);
                var ThisAction = Action.Поиграть;
                var PetScanning = await PetScan(ThisAction, GetPet, user);
                var emb = PetScanning.Item1;
                await Context.Channel.SendMessageAsync("", false, emb.Build());

                if (PetScanning.Item2)
                    await RandomMessage(emb, user, GetPet.Name, ThisAction);
            }
        }

        private async Task<(EmbedBuilder, bool)> PetScan(Action Act, Pets ThisPet, SocketGuildUser User)
        {
            using (db _db = new ())
            {
                bool success = false;
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Питомец ");
                if (ThisPet == null || ThisPet.Die)
                {
                    if (ThisPet != null && ThisPet.Die)
                        PetService.PetDie(ThisPet, User);
                    emb.Author.Name += "отсустствует!";
                    emb.WithDescription($"У {(User.Id != Context.User.Id ? "данного пользователя" : "вас")} еще нету своего любимца!");
                }
                else
                {
                    byte Max = 0;
                    string Text1 = string.Empty;
                    string Text2 = string.Empty;
                    var Type = PetItemEnum.Еда;
                    if (Act == Action.Вылечить)
                    {
                        Max = ThisPet.HP;
                        Text1 = "помощи";
                        Text2 = "медикоменты";
                        Type = PetItemEnum.МедПомощь;
                    }
                    else if (Act == Action.Поиграть)
                    {
                        Max = ThisPet.MOOD;
                    }
                    else
                    {
                        Max = ThisPet.EAT;
                        Text1 = "кормлении";
                        Text2 = "еды";
                        Type = PetItemEnum.Еда;
                    }

                    if (Max == ThisPet.MaxHaracter && Act != Action.Поиграть)
                        emb.WithDescription($"Питомец не нуждается в {Act}.");
                    else
                    {
                        emb.Author.Name += ThisPet.Name;

                        Items Item = null;

                        if (Act != Action.Поиграть)
                        {
                            if (User.Id == Context.User.Id)
                                Item = ThisPet.Items.FirstOrDefault(x => x.ItemType == Type);
                            else
                                Item = _db.Pets.Include(x => x.Items).FirstOrDefault(x => x.UserId == User.Id)?.Items.FirstOrDefault(x => x.ItemType == Type);
                        }

                        if(Act == Action.Поиграть || Act != Action.Поиграть && Item != null)
                        {
                            success = true;
                            emb = await PetAction(Act, User, emb, ThisPet, Item);
                        }
                        else
                        {
                            var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                            emb.WithDescription($"Для того чтобы помочь, купите питомцу {Text2}!").WithFooter($"{Prefix}PetShop");
                        }

                    }
                }
                return (emb, success);
            }
        }

        private async Task RandomMessage(EmbedBuilder emb, SocketUser user, string Petname, Action Type)
        {
            int rnd = new Pcg.PcgRandom(1488).Next(0, 10);
            if (user == Context.User && rnd >= 6)
            {
                rnd = new Pcg.PcgRandom(1488).Next(1, 9);
                string text = string.Empty;
                switch (Type)
                {
                    case Action.Покормить:
                        switch (rnd)
                        {
                            case 1:
                                text = "Спасибо что покормил меня... Я тебя очень очень люблю.";
                                break;
                            case 2:
                                text = "Ты сегодня такой красивый... А я испачкался. Вытрешь мне мордочку?";
                                break;
                            case 3:
                                text = "Ура ура ура... Хозяин, как твои дела? Надеюсь, так же отлично, как теперь и у меня!";
                                break;
                            case 4:
                                text = "Хозяин, а на мое день рождения мы купим большой тортик? Я очень хочу тортик...";
                                break;
                            case 5:
                                text = "Хозяин, надеюсь ты не злишься, что я все так быстро сьел? Мне все понравилось!";
                                break;
                            case 6:
                                text = "Это все Мне? Я очень рад что у меня такой, хороший хозяин. Я тебя люблю!";
                                break;
                            case 7:
                                text = "Я бы очень хотел покушать с тобой как раньше. Было очень вкусно!";
                                break;
                            case 8:
                                text = "Ты такой милый... Если тебе поднимает настроение что я кушаю, я готов есть еще!";
                                break;
                        }
                        break;
                    case Action.Поиграть:

                        switch (rnd)
                        {
                            case 1:
                                text = "Хозяин... Хозяин, ты такой хороший, я так рад что ты у меня есть... Я тебя очень очень люблю.";
                                break;
                            case 2:
                                text = "Я понимаю что я не смогу заменить реального друга... Но надеюсь ты мне все равно любишь...";
                                break;
                            case 3:
                                text = "Тебе нравится со мной играться? Если это приносит удовольствие, можешь делать чаще.";
                                break;
                            case 4:
                                text = "Ура ура, спасибо что уделяешь мне время, я очень рад этому.";
                                break;
                            case 5:
                                text = "Ты такой милый, поиграй со мной еще, а я посмотрю на тебя.";
                                break;
                            case 6:
                                text = "Почеши еще за ушком пожалуйста.";
                                break;
                            case 7:
                                text = "Почешишь мне животик?";
                                break;
                            case 8:
                                text = "А какая твоя любимая игрушка хозяин? Я очень люблю бегать за своим хвостом.";
                                break;
                        }

                        break;
                    case Action.Вылечить:

                        switch (rnd)
                        {
                            case 1:
                                text = "Спасибо что вылечил меня, мой любимый хозяин!";
                                break;
                            case 2:
                                text = "Я понимаю что я не смогу заменить реального друга... Но надеюсь ты мне все равно любишь...";
                                break;
                            case 3:
                                text = "Тебе нравится со мной играться? Если это приносит удовольствие, можешь делать чаще.";
                                break;
                            case 4:
                                text = "Так так добр ко мне, спасибо!";
                                break;
                            case 5:
                                text = "Ты такой милый... Хочется тебя обнять, чтобы ты никогда не грустил.";
                                break;
                            case 6:
                                text = "Твоя помощь очень делает тепло, в моем маленьком сердечке...";
                                break;
                            case 7:
                                text = "Сколько стоят эти таблеточки, я смогу расплатиться, если помурчку на ушко?";
                                break;
                            case 8:
                                text = "Хозяин, спасибо что тратишься на меня!";
                                break;
                        }

                        break;
                }


                emb.WithAuthor($"**{Petname} радуется**").WithDescription(text);
                await Task.Delay(1000);
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task petshop(sbyte NumberItem = 0)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Магазин");
                var ShopItems = _db.Items.Where(x => x.PetsId == 1337);
                if (ShopItems.Any())
                {
                    if (NumberItem == 0)
                    {
                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        int i = 1;
                        foreach (var Item in ShopItems)
                        {
                            emb.AddField($"{i}.{Item.Name}{Item.Emoji}", $"Цена: {Item.Price}\n{(Item.ItemType == PetItemEnum.Еда ? "Сытость:" : "Лечение:")} +{Item.Value}", true);
                            i++;
                        }
                        emb.WithFooter($"Купить {Prefix}PetShop [Номер]");
                    }
                    else
                    {
                        if (NumberItem == 0)
                            NumberItem = 1;
                        var Item = ShopItems.AsEnumerable().ElementAt(NumberItem - 1);
                        if (Item == null)
                            emb.WithDescription("Элемент под таким номером не найден!");
                        else
                        {
                            var User = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id); ;
                            if (User.ZeroCoin >= Item.Price)
                            {
                                User.ZeroCoin -= Item.Price;
                                var GetPet = _db.Pets.FirstOrDefault(x => x.UserId == Context.User.Id);
                                Items NewItem = new() { Emoji = Item.Emoji, ItemType = Item.ItemType, Name = Item.Name, PetsId = GetPet.Id, Price = Item.Price, Value = Item.Value };
                                _db.Items.Add(NewItem);
                                _db.Users_Guild.Update(User);
                                await _db.SaveChangesAsync();
                                emb.WithDescription($"Вы успешно купили {Item.Name} за {Item.Price}!");
                            }
                            else
                                emb.WithDescription($"У вас недостаточно денег на счету: {User.ZeroCoin}.");
                        }
                    }
                }
                else
                    emb.WithDescription("На данный момент в магазине нету вещей!");

                await Context.Channel.SendMessageAsync("", false, emb.Build());

            }
        }

        private static byte MaxDefect(byte A, byte B,byte Level) => (byte)Math.Clamp(A + B, byte.MinValue, 127 + Level);

        private enum Action
        {
            Покормить,
            Поиграть,
            Вылечить,
        }
        private async Task<EmbedBuilder> PetAction(Action Act, SocketUser User, EmbedBuilder emb, Pets ThisPet, Items Item = null)
        {
            using (db _db = new ())
            {
                string text = string.Empty;
                string text2 = string.Empty;
                string text3 = string.Empty;
                var Max = ThisPet.MaxHaracter;
                if (ThisPet.SleepNow)
                {
                    ThisPet.SleepNow = false;
                    text3 = "**разбудили** и";
                }

                switch (Act)
                {
                    case Action.Покормить:
                        ThisPet.EAT = MaxDefect(ThisPet.EAT, (byte)Item.Value, ThisPet.Level);
                        ThisPet.XP += 10;
                        text = $"Вы {text3} покормили питомца";
                        text2 = $"Сытость: {ThisPet.EAT}/{Max}";
                        break;
                    case Action.Поиграть:
                        
                        text = $"Вы {text3} поигрались с питомцем";
                        byte Count = (byte)new Pcg.PcgRandom(1488).Next(1, 10);
                        if(ThisPet.MOOD == ThisPet.MaxHaracter)
                            ThisPet.XP += 1;
                        else
                            ThisPet.XP += 5;

                        ThisPet.MOOD = MaxDefect(ThisPet.MOOD, Count, ThisPet.Level);

                        ThisPet.SLEEP -= 1;
                        text2 = $"Настроение: {ThisPet.MOOD}/{Max}[+{Count}]\nСон: {ThisPet.SLEEP}/{Max}[-1]";
                        break;
                    case Action.Вылечить:
                        ThisPet.XP += 10;
                        text = $"Вы {text3} вылечили питомца";
                        ThisPet.EAT = MaxDefect(ThisPet.EAT, (byte)Item.Value, ThisPet.Level);
                        ThisPet.MOOD = MaxDefect(ThisPet.MOOD, (byte)Item.Value, ThisPet.Level);
                        ThisPet.SLEEP = MaxDefect(ThisPet.SLEEP, (byte)Item.Value, ThisPet.Level);
                        text2 = $"Здоровье: {ThisPet.HP}/{Max}";
                        break;
                }

                if (Item != null)
                    _db.Items.Remove(Item);

                _db.Pets.Update(ThisPet);
                await _db.SaveChangesAsync();
                if (User == Context.User)
                    emb.WithDescription($"{text}\n{text2}");
                else
                    emb.WithDescription($"{text} {User.Mention}\n\n{text2}").WithFooter("Спасибо что позаботился обо мне <3");

                return emb;
            }
        }
    }
}
