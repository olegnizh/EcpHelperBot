using EcpHelperBot.Configuration;
using EcpHelperBot.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using Telegram.Bot;
using Telegram.Bot.Types;
//using static System.Net.Mime.MediaTypeNames;
//using static System.Net.WebRequestMethods;



namespace EcpHelperBot.Controllers
{

    public class DefaultMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;
        private readonly IFileHandler _imageFileHandler;
        private readonly IOperation _operation;
        private readonly AppSettings _appSettings;

        public DefaultMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage, IFileHandler imageFileHandler, IOperation operation, AppSettings appSettings)        
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
            _imageFileHandler = imageFileHandler;
            _operation = operation;
            _appSettings = appSettings;

        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            //if (_memoryStorage.GetSession(message.Chat.Id).Operation == "no")
            if (_memoryStorage.GetSession(message.Chat.Id).NumApplication == "")
            {               
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, " Заявка по файлу не определена - не могу обработать", cancellationToken: ct);
                return;
            }

            var fileId = message.Photo.Last()?.FileId;
            if (fileId == null)
                return;

            Console.WriteLine($"Контроллер {GetType().Name} получил сообщение - photoid = " + message.Photo.Last().FileId);
            
            _memoryStorage.GetSession(message.Chat.Id).Count = _memoryStorage.GetSession(message.Chat.Id).Count + 1;
            string image = message.From.Id.ToString() + "-" + _memoryStorage.GetSession(message.Chat.Id).Count.ToString();
            await _imageFileHandler.Download(fileId, ct, image);

            await _telegramClient.SendPhotoAsync(chatId: message.Chat.Id, photo: InputFile.FromFileId(message.Photo.Last().FileId), caption: _memoryStorage.GetSession(message.Chat.Id).SubjectMessage);
            await _telegramClient.SendPhotoAsync(chatId: -4116444453, photo: InputFile.FromFileId(message.Photo.Last().FileId), caption: _memoryStorage.GetSession(message.Chat.Id).SubjectMessage);

            await _imageFileHandler.SendToEmailImage(image, _memoryStorage.GetSession(message.Chat.Id).SubjectMessage, _appSettings.EmailFrom, _appSettings.EmailTo, _appSettings.ServSmtp, _appSettings.LoginEmail, _appSettings.PasswordEmail);


        }

    }
}