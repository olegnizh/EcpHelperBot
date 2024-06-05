using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using EcpHelperBot.Services;
using EcpHelperBot.Configuration;
using EcpHelperBot.Controllers;
using EcpHelperBot.Extensions;
using EcpHelperBot.Context;
//using Microsoft.EntityFrameworkCore;
using MySqlConnector;


namespace EcpHelperBot
{
    public class Program
    {

        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            // Объект, отвечающий за постоянный жизненный цикл приложения
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
                .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                .Build(); // Собираем

            Console.WriteLine("Сервис запущен");
            NlogExtension.logger.Info("Сервис запущен", DateTime.Now);
            // Запускаем сервис
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");
            NlogExtension.logger.Info("Сервис остановлен", DateTime.Now);

        }

        static void ConfigureServices(IServiceCollection services)
        {

            AppSettings appSettings = BuildAppSettings();
            services.AddSingleton(BuildAppSettings());

            services.AddTransient<AppDb>(_ => new AppDb(appSettings.ConnectionString));

            services.AddSingleton<IOperation, Operation>();
            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<IFileHandler, ImageFileHandler>();

            // Подключаем контроллеры сообщений и кнопок
            services.AddTransient<DefaultMessageController>();
            services.AddTransient<VoiceMessageController>();
            services.AddTransient<TextMessageController>();
            services.AddTransient<InlineKeyboardController>();

            // Регистрируем объект TelegramBotClient c токеном подключения
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));
            
            // Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();

        }

        static AppSettings BuildAppSettings()
        {
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var connectionString = MyConfig.GetValue<string>("ConnectionString:DefaultConnection");
            var botToken = MyConfig.GetValue<string>("AppSettings:BotToken");
            var emailFrom = MyConfig.GetValue<string>("AppSettings:EmailFrom");
            var emailTo = MyConfig.GetValue<string>("AppSettings:EmailTo");
            var servSmtp = MyConfig.GetValue<string>("AppSettings:ServSmtp");
            var loginEmail = MyConfig.GetValue<string>("AppSettings:LoginEmail");
            var passwordEmail = MyConfig.GetValue<string>("AppSettings:PasswordEmail");
            var questAnsw = MyConfig.GetValue<string>("AppSettings:QuestAnsw");
            var downloadsFolder = MyConfig.GetValue<string>("AppSettings:DownloadsFolder");
            var imageFileName = MyConfig.GetValue<string>("AppSettings:ImageFileName");

            return new AppSettings()
            {
                ConnectionString = connectionString,
                BotToken = botToken,
                EmailFrom = emailFrom,
                EmailTo = emailTo,
                ServSmtp = servSmtp,
                LoginEmail = loginEmail,
                PasswordEmail = passwordEmail,
                QuestAnsw = questAnsw,
                DownloadsFolder = downloadsFolder,
                ImageFileName = imageFileName

            };
        }


    }

}