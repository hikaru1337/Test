using DarlingDb;
using DarlingDb.Models;
using DarlingNet.Services.LocalService.Errors;
using DarlingNet.Services.LocalService.GetOrCreate;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService
{
    public class ErrorMessage
    {
        public static async Task<EmbedBuilder> GetError(string error, string prefix, CommandInfo command = null)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Ошибка!");
                string text = string.Empty;
                var SetError = Initiliaze.Load(error);

                if (SetError?.Rus != error)
                {

                    if (SetError.HelpCommand == "true") //  || error.Contains("Value is not a ") 
                    { 
                        foreach (var Parameter in command.Parameters)
                        {
                            if (Parameter.IsOptional)
                                text += $"[{Parameter}/может быть пустым]";
                            else if (Parameter.IsRemainder)
                                text += $"[{Parameter}/поддерживает предложения]";
                            else
                                text += $"[{Parameter}] ";
                        }

                        emb.WithDescription($"Описание ошибки: " + SetError.Rus);
                        emb.AddField($"Описание команды: ", $"{command.Summary ?? "отсутствует"}", true);
                        emb.AddField("Пример команды:", $"{prefix}{command.Name} {text}");
                    }
                    else
                        emb.WithDescription($"{SetError.Rus}");
                }
                else
                {
                    emb.WithDescription($"{error}");
                    if (!error.Any(x => x >= 1040 && x <= 1103))
                    {
                        emb.Description += "\n\nОшибка отправлена на перевод, спасибо за вашу помощь!";
                        Feedback ThisFeedBack = _db.Feedback.FirstOrDefault(x => x.Message == $"+TRANSLATE={error}" || x.Message.Contains(error));
                        if (ThisFeedBack == null)
                        {
                            ThisFeedBack = new Feedback { UserId = BotSettings.hikaruid, Message = $"+TRANSLATE={error}", Time = DateTime.Now, Status = StatusTicketEnum.Отправлен };
                            _db.Feedback.Add(ThisFeedBack);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                return emb;
            }
        }
    }
}
