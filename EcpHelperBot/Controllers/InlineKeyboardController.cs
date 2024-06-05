using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using EcpHelperBot.Services;
using Telegram.Bot.Types.ReplyMarkups;


namespace EcpHelperBot.Controllers
{

    public class InlineKeyboardController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            // Обновление пользовательской сессии новыми данными
            _memoryStorage.GetSession( callbackQuery.From.Id ).Operation = callbackQuery.Data;
            Console.WriteLine( "Handle - Выбрана операция = " + _memoryStorage.GetSession( callbackQuery.From.Id).Operation );

            // Генерим информационное сообщение
            string OperationDesc = callbackQuery.Data switch
            {
                "bt1" => " bt1",
                "bt2" => " bt2",
                _ => String.Empty
            };

            // Отправляем в ответ уведомление о выборе
            await _telegramClient.SendTextMessageAsync(
                       chatId: callbackQuery.From.Id,
                       text: $"<b>Выбрано - </b>{OperationDesc}.{Environment.NewLine}",
                       //replyMarkup: new ReplyKeyboardRemove(),
                       replyMarkup: null,
                       cancellationToken: ct,
                       parseMode: ParseMode.Html);
            /*
            if ( callbackQuery.Data == "endwork" )
               _memoryStorage.GetSession( callbackQuery.From.Id ).Flag = 0;
            else
               _memoryStorage.GetSession( callbackQuery.From.Id ).Flag = 1;
            */
        }

    }
}