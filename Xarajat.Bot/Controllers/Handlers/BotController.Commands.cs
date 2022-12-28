using Xarajat.Bot.Entities;

namespace Xarajat.Bot.Controllers;

public partial class BotController
{
    private async void HandleCommandMessage(User user, string message)
    {
        if (user.Step != 4)
        {
            if (message == "/key")
            {
                if (user.IsAdmin)
                {
                    _botService.SendMessage(user.ChatId, $"Xona kaliti 👇 \n");
                    _botService.SendMessage(user.ChatId, user.Room.Key);
                }
                else
                {
                    _botService.SendMessage(user.ChatId, "Kalitni admindan so'rang");
                }
            }
            else if (message == "/users")
            {
                if (user.Step < 3)
                {
                    _botService.SendMessage(user.ChatId, "Foydalanuvchilar royxatini bilish uchun avval xonaga kiring");
                }
                else
                {
                    string users = "";
                    foreach (var u in user.Room.Users)
                    {
                        users += u.Fullname + ", \n";
                    }

                    _botService.SendMessage(user.ChatId, users);
                }
            }
            else if (message == "/myroom")
            {
                if (user.Step < 3)
                {
                    _botService.SendMessage(user.ChatId, "Xona nomini bilish uchun avval  xonaga kiring yoki yangi xona oching");
                }
                else
                {
                    _botService.SendMessage(user.ChatId, user.Room.Name);
                }

            }
            else if (message == "/outofroom")
            {
                if (user.Step < 3)
                {
                    _botService.SendMessage(user.ChatId, "Siz xonada emassiz");
                }
                else
                {
                    var wordList = new List<string> { "Ha chiqaman", "Yo'q qolaman" };
                    _botService.SendMessage(user.ChatId, "Haqiqatdan ham xonadan chiqmoqchimisiz", _botService.GetInlineKeyboard(wordList));
                }
            }

            else if (message == "Ha chiqaman")
            {
                if (user.IsAdmin)
                {
                    var room = user.Room;
                    room.Status = RoomStatus.Finished;
                    await _roomRepository.UpdateRoom(room);
                }

                await _userRepository.DeleteUSer(user);

                var menu = new List<string>() { "🏠Yangi xona ochish", "🏠🏃‍♂️Xonaga qo'shilish" };

                _botService.SendMessage(user.ChatId, "Siz xonadan chiqtingiz");
                _botService.SendMessage(user.ChatId, "Menu", _botService.GetKeyboard(menu));
            }
            else if (message == "Yo'q qolaman")
            {
                if (user.Step == 3)
                {
                    var menu = new List<string>() { "🛒Xarajat qo'shish", "📊Umumiy xarajatlar", "👤💸Mening xarajatlarim" };
                    _botService.SendMessage(user.ChatId, "Xonada qoldingiz", _botService.GetKeyboard(menu));
                }
                else
                {
                    _botService.SendMessage(user.ChatId, "Xonada qoldingiz");
                }
            }
        }
    }
}
