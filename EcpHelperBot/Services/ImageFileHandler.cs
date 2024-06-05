using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.IO;
using System.Threading;
using EcpHelperBot.Configuration;
using EcpHelperBot.Services;
using MimeKit.Utils;
using MimeKit;
using MailKit.Net.Smtp;
//using Telegram.Bot.Types;


namespace EcpHelperBot.Services
{
    public class ImageFileHandler : IFileHandler
    {
        private readonly AppSettings _appSettings;
        private string _imageFilePath { get; }
        private readonly ITelegramBotClient _telegramBotClient;

        public ImageFileHandler(ITelegramBotClient telegramBotClient, AppSettings appSettings)
        {
            
            _appSettings = appSettings;
            //_imageFilePath = Path.Combine(_appSettings.DownloadsFolder, $"{_appSettings.ImageFileName}{DateTime.Now.ToString("yyMMddHHss")}");
            //_imageFilePath = Path.Combine(_appSettings.DownloadsFolder, $"{_appSettings.ImageFileName}");
            _telegramBotClient = telegramBotClient;

        }

        public async Task Download(string fileId, CancellationToken ct, string imageid)
        {
            // Генерируем полный путь файла из конфигурации
            //string inputImageFilePath = Path.Combine($"{_imageFilePath}.{_appSettings.InputAudioFormat}");

            //using (FileStream destinationStream = File.Create(_imageFilePath + "-" + imageid + ".jpg"))
            using (FileStream destinationStream = File.Create(_appSettings.DownloadsFolder + imageid + ".jpg"))
            {
                // Загружаем информацию о файле
                var file = await _telegramBotClient.GetFileAsync(fileId, ct);
                if (file.FilePath == null)
                    return;
                // Скачиваем файл
                await _telegramBotClient.DownloadFileAsync(file.FilePath, destinationStream, ct);
            }

        }

        public async Task SendToEmailImage(string imageid, string subject, string from, string to, string servsmtp, string login, string password)
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();
            //var image = builder.LinkedResources.Add(@"D:\\cvets.jpg");
            var image = builder.LinkedResources.Add(_appSettings.DownloadsFolder + imageid + ".jpg");
            image.ContentId = MimeUtils.GenerateMessageId();
            builder.HtmlBody = string.Format(@"<p> </p><img src=""cid:{0}"">", image.ContentId);
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(servsmtp, 587);
            smtp.Authenticate(login, password);
            await smtp.SendAsync(email);
            //smtp.Send(email);
            smtp.Disconnect(true);

        }


    }

}
