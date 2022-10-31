using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xarajat.Bot.Repositories;
using Xarajat.Bot.Services;
using Xarajat.Data.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Update = Telegram.Bot.Types.Update;
using User = Xarajat.Data.Entities.User;


namespace Xarajat.Bot.Controllers;

[Route("bot")]
[ApiController]
public partial class BotController : ControllerBase
{
    private readonly TelegramBotService _botService;
    private readonly UserRepository _userRepository;
    private readonly OutlayRepository _outlayRepository;
    private readonly RoomRepository _roomRepository;
    private readonly ILogger<BotController> _logger;

    public BotController(TelegramBotService service,
                         UserRepository userRepository,
                         OutlayRepository outlayRepository,
                         RoomRepository roomRepository,
                         ILogger<BotController> logger)
    {
        _botService = service;
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _outlayRepository = outlayRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetMe() => Ok("bot ishladi");

    [HttpPost]
    public async Task PostUpdate(Update update)
    {


        if (update.Type != UpdateType.Message && update.Type != UpdateType.CallbackQuery)
            return;


        var (chatId, message, username) = GetValues(update);
        var user = await FilterUser(chatId, username);



        if (message is "/key" or "/users" or "/myroom" or "/outofroom") //  or "Ha chiqaman" or "Yo'q qolaman"
        {
            _logger.LogInformation($" mami data kere -> {message}");
            //botdagi menu  commandalari
            HandleCommandMessage(user, message);
        }
        else if (user.Step == 0)
        {
            if (message == "Yangi xona ochish")
            {
                user.Step = 1;
                await _userRepository.UpdateUser(user);
                _botService.SendMessage(user.ChatId, "Xonani nomini kiriting");
            }
            else if (message == "Xonaga qo'shilish")
            {
                user.Step = 2;
                await _userRepository.UpdateUser(user);
                _botService.SendMessage(user.ChatId, "kalit so'zni kiriting");
            }
            else
            {
                var menu = new List<string>() { "Yangi xona ochish", "Xonaga qo'shilish" };
                _botService.SendMessage(user.ChatId, "Menu", _botService.GetKeyboard(menu));
            }
        }
        else if (user.Step == 1)
        {
            var room = new Room
            {
                Name = message,
                Key = Guid.NewGuid().ToString("N")[..10],
                Status = RoomStatus.Created
            };

            await _roomRepository.AddRoomAsync(room);

            _botService.SendMessage(user.ChatId, $" Xonaga xush kelibsiz"); //{user.Room.Name}

            user.RoomId = room.Id;
            user.IsAdmin = true;
            user.Step = 3;
            await _userRepository.UpdateUser(user);

            var menu = new List<string>() { "Xarajat qo'shish" };
            _botService.SendMessage(user.ChatId, _botService.GetKeyboard(menu));

        }
        else if (user.Step == 2)
        {

            var room = await _roomRepository.GetRoomByKey(message);

            if (room is null)
            {
                _botService.SendMessage(user.ChatId, "Unday xona topilmadi , kalit so'zi qayta kiriting");
            }
            else if (room is not null)
            {
                user.RoomId = room.Id;
                user.Step = 3;
                await _userRepository.UpdateUser(user);

                _botService.SendMessage(user.ChatId, $"{room.Name} nomli xonaga  xush kelibsiz. \n Xona xaqida batafsil malumotlar menuda");

                var menu = new List<string>() { "Xarajat qo'shish" };
                _botService.SendMessage(user.ChatId, _botService.GetKeyboard(menu));
            }
        }
        else if (user.Step == 3)
        {
            if (user.Room.Status is not RoomStatus.Active)
            {
                var room = user.Room;
                room.Status = RoomStatus.Active;
                await _roomRepository.UpdateRoom(room);
            }

            if (message == "Xarajat qo'shish")
            {

                user.Step = 4;
                await _userRepository.UpdateUser(user);
                _botService.SendMessage(user.ChatId, "xarajatingizni quyidagi tartibda kiriting: avval summasi keyin nimaligi yozib yuboorish tugmasini bosing 👇 \n");
                _botService.SendMessage(user.ChatId, "15000 - olma oldim \nyoki \n40000 - yarim kilo go'sht oldim");
            }
            else if (message == "Mening xarajatlarim")
            {
                string outlaysList = "";
                int sum = 0;

                var userOutlayList = user.Outlays;

                foreach (var outlay in userOutlayList)
                {
                    outlaysList += outlay.Cost + " - " + outlay.Description + "\n\n";
                    sum += outlay.Cost;
                }

                _botService.SendMessage(chatId, outlaysList);
                _botService.SendMessage(user.ChatId, $"Mening xarajatim {sum} sum");

            }
            else if (message == "Umumiy xarajatlar")
            {
                string outlaysList = "";
                int sum = 0;

                var room = user.Room;

                foreach (var outlay in room.Outlays)
                {
                    outlaysList += outlay.ToReadable + "\n\n";
                    sum += outlay.Cost;
                }

                _botService.SendMessage(chatId, outlaysList);
                _botService.SendMessage(user.ChatId, $"Umumiy xarajat {sum} sum");
            }
            else
            {
                var menu = new List<string>() { "Xarajat qo'shish", "Umumiy xarajatlar", "Mening xarajatlarim" };
                _botService.SendMessage(user.ChatId, "🤔", _botService.GetKeyboard(menu));
            }
        }
        else if (user.Step == 4)
        {
            if (string.IsNullOrEmpty(message))
            {
                _botService.SendMessage(user.ChatId, "Iltimos xarajatni korsatilgan tartibga mos ravishda kiriting");
            }
            else
            {
                var outlaysArray = message.Split('-').ToArray();

                int.TryParse(outlaysArray[0], out var cost);

                if (outlaysArray.Length != 2 || cost == 0)
                {
                    _botService.SendMessage(user.ChatId, "Iltimos xarajatni korsatilgan tartibga mos ravishda kiriting");
                }
                else
                {
                    //xarajatlarni qo'wiw 

                    var outlay = new Outlay
                    {
                        Cost = cost,
                        Description = outlaysArray[1],
                        UserId = user.Id,
                        RoomId = user.RoomId
                    };

                    await _outlayRepository.AddOutlayAsync(outlay);

                    user.Step = 3;
                    await _userRepository.UpdateUser(user);

                    var menu = new List<string>() { "Xarajat qo'shish", "Umumiy xarajatlar", "Mening xarajatlarim" };
                    _botService.SendMessage(user.ChatId, "✅ Xarajatingiz muvaffaqiyatli qo'shildi", _botService.GetKeyboard(menu));
                }
            }
        }
    }

    //private async Task HandleUnknownUpdate(TelegramBotService botService, Update update)
    //{
    //    _botService.SendMessage(update.Message.Chat.Id, "unknown type");
    //}

    //private async Task HandleCallbackQueryAsync(TelegramBotService botService, CallbackQuery? callbackQuery, Update update)
    //{
    //    if(callbackQuery.Data == "Ha chiqaman")
    //    {
    //        _botService.SendMessage(update.Message.Chat.Id, "chiqti");
    //    }

    //}

    //private async Task HandleMessageAsync(TelegramBotService botService, Message? message)
    //{

    //}

    private Tuple<long, string, string> GetValues(Update update)
    {

        long chatId = 0;
        var message = "";
        var name = "";
        if (update.Type == UpdateType.Message)
        {
            chatId = update.Message!.From!.Id;
            message = update.Message.Text!;
            name = update.Message.From.Username ?? update.Message.From.FirstName;
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            chatId = update.CallbackQuery!.From!.Id;
            message = update.CallbackQuery.Data;
            name = update.CallbackQuery.From.Username ?? update.CallbackQuery.From.FirstName;
        }

        return new(chatId, message, name);
    }

    private async Task<User> FilterUser(long chatId, string username)
    {
        var user = await _userRepository.GetUserByChatId(chatId);

        if (user is null)
        {
            user = new User()
            {
                ChatId = chatId,
                Name = username,
                CreatedDate = DateTime.Now,
            };

            await _userRepository.AddUserAsync(user);
        }
        return user;
    }

}

