using DarlingDb;
using DarlingDb.Models;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Discord.WebSocket;

namespace DarlingNet.Services.LocalService
{
    internal class PetService
    {
        public async static void PetDie(Pets Pet,SocketGuildUser User)
        {
            using (db _db = new ())
            {
                if(Pet.Id != 1337)
                {
                    _db.Pets.Remove(Pet);
                    await _db.SaveChangesAsync();
                    var emb = new EmbedBuilder().WithColor(Color.DarkRed).WithAuthor($"**{Pet.Name} умер**").WithDescription("Прости меня мой хозяин... Я не смог выжить без твоей любви, заботы, и тебя... Ты для меня был очень дорог, но ты не смог прийти когда я умирал...\n\nНадеюсь ты найдешь себе лучше друга чем я... Прощай, ты был единственным другом в моей короткой жизни...");
                    await User.UserMessage("", emb);
                }
            }
        }
    }
}
