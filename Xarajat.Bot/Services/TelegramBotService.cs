using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;

namespace Xarajat.Bot.Services;

public class TelegramBotService
{
    private readonly TelegramBotClient _bot;

    public TelegramBotService(IConfiguration configuration)
    {
        _bot = new TelegramBotClient(configuration["BotToken"]);
    }

    public void SendMessage(long chatId, string message, IReplyMarkup reply = null)
    {
        _bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
    }
    public void SendMessage(long chatId,  IReplyMarkup reply = null)
    {
        _bot.SendTextMessageAsync(chatId, " ", replyMarkup: reply);
    }

    public ReplyKeyboardMarkup GetKeyboard(List<string> buttonsText)
    {
        var buttons = new KeyboardButton[buttonsText.Count][];

        for (var i = 0; i < buttonsText.Count; i++)
        {
            buttons[i] = new KeyboardButton[] { new(buttonsText[i]) };
        }

        return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
    }

    public InlineKeyboardMarkup GetInlineKeyboard(List<string> buttonsText, int? correctAnswerIndex = null, int? questionIndex = null)
    {
        InlineKeyboardButton[][] buttons = new InlineKeyboardButton[buttonsText.Count][];

        for (var i = 0; i < buttonsText.Count; i++)
        {
            buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(
                text: buttonsText[i],
                callbackData: correctAnswerIndex == null ? buttonsText[i] : $"{correctAnswerIndex},{i},{questionIndex}"),  };
        }

        return new InlineKeyboardMarkup(buttons);
    }
}

