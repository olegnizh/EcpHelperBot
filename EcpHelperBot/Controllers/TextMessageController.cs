using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using EcpHelperBot.Services;
using System.Data;
using EcpHelperBot.Configuration;
using EcpHelperBot.Extensions;
using EcpHelperBot.Context;
using EcpHelperBot.Models;
using MySqlConnector;
using Humanizer;


namespace EcpHelperBot.Controllers
{

    public class TextMessageController
    {

        private readonly ITelegramBotClient _telegramClient;
        private readonly IOperation _operation;
        private readonly IStorage _memoryStorage;
        private readonly AppSettings _appSettings;
        private readonly AppDb db;

        DataSet ds;    
        List<string> ListQuest = new List<string>();
        
        ReplyKeyboardMarkup rkm;
        string[] KeyboardButton;

        public TextMessageController(ITelegramBotClient telegramBotClient, IOperation operation, IStorage memoryStorage, AppSettings appSettings, AppDb appDb)
        {
            _telegramClient = telegramBotClient;
            _operation = operation;
            _memoryStorage = memoryStorage;
            _appSettings = appSettings;
            db = appDb;

            this.ds = new DataSet();
            this.InitQuestMenu();

        }

        private void InitQuestMenu()
        {
            this.ds.Clear();

            //this.ds.ReadXml( _appSettings.QuestAnsw + ".xml" );

            this.db.Connection.Open();
            var cmd = this.db.Connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Qas ORDER BY Quest;";
            cmd.ExecuteNonQuery();
            var sAdapter = new MySqlDataAdapter(cmd);
            sAdapter.Fill(this.ds, "Qas");
            DataTable dt = this.ds.Tables["Qas"];
            this.db.Connection.Close();

            // актуальный список вопросов
            this.ListQuest.Clear();
            foreach (DataRow row in this.ds.Tables[_appSettings.QuestAnsw].Rows)
                this.ListQuest.Add(row["Quest"].ToString());

            this.KeyboardButton = this.ListQuest.ToArray();

        }

        public async void QuestionAsync( Message message, CancellationToken ct )
        {

            DataRow[] resultd;
            string sss = "";            

            _memoryStorage.GetSession(message.Chat.Id).Operation = "no";
            _memoryStorage.GetSession( message.Chat.Id ).Quest = message.Text;
            _memoryStorage.GetSession(message.Chat.Id).Count = 0;
            _memoryStorage.GetSession(message.Chat.Id).NumApplication = "";

            Console.WriteLine( "QuestionAsync - select question = " + message.Text );
            NlogExtension.logger.Info("Text = " + message.Text, DateTime.Now);

            resultd = this.ds.Tables[ _appSettings.QuestAnsw ].Select( "Quest = '" + _memoryStorage.GetSession( message.Chat.Id ).Quest + "'" );
            if ( resultd[0]["Answ"].ToString() == "" )
                sss = "На вопрос ответа пока нет";
            else
            {
                sss = resultd[ 0 ][ "Answ" ].ToString();
                if ( resultd[ 0 ][ "Hyper" ].ToString() != "" )
                   sss = sss + "\n <a href=\"" + resultd[ 0 ][ "Hyper" ].ToString() + "\">Ссылка</a>";
            }

            this.rkm = new( new[] { new KeyboardButton[] { "Вернуться к вопросам", "Написать в техподдержку", "Отмена" } } )
            { ResizeKeyboard = true };
            await _telegramClient.SendTextMessageAsync(
                     chatId: message.Chat.Id,
                     text: sss,
                     cancellationToken: ct,
                     parseMode: ParseMode.Html,
                     replyMarkup: rkm);
            return;

        }

        public async Task Handle( Message message, CancellationToken ct )
        {
            if (message.Text == "/bt1")
            {
                var buttons = new List<InlineKeyboardButton[]>();
                buttons.Add(new[]
                {
                        InlineKeyboardButton.WithCallbackData($"Кнопка 1" , $"bt1"),
                        InlineKeyboardButton.WithCallbackData($"Кнопка 2" , $"bt2"),
                        InlineKeyboardButton.WithCallbackData($"Кнопка 3" , $"bt3")
                });
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Экранные кнопки", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));
                return;
            }
           
            if ((message.Text == "/start") || (message.Text == "Отмена"))
            {
                
                _memoryStorage.GetSession(message.Chat.Id).Operation = "no";
                _memoryStorage.GetSession(message.Chat.Id).Count = 0;
                _memoryStorage.GetSession(message.Chat.Id).NumApplication = "";

                this.rkm = new( new[] { new KeyboardButton[] { "Выбрать вопрос по работе в ЕЦП", "Получить статус по заявке" } } )
                { ResizeKeyboard = true };
                await _telegramClient.SendTextMessageAsync(
                     chatId: message.Chat.Id,
                     text: $"<b>Наш бот помогает техподдержке ЕЦП\nПосмотрите имеющиеяся вопросы и если найдете - выберите вопрос по проблеме\nЕсли не найдете - нужно написать в техподдержку для оформления заявки</b>",
                     cancellationToken: ct,
                     parseMode: ParseMode.Html,
                     replyMarkup: rkm);
                return;
            }

            if (message.Text == "Получить статус по заявке")
            {
                _memoryStorage.GetSession(message.Chat.Id).Operation = "worked";
                _memoryStorage.GetSession(message.Chat.Id).Count = 0;
               

                this.rkm = new(new[] { new KeyboardButton[] { "Отмена" } })
                { ResizeKeyboard = true };
                await _telegramClient.SendTextMessageAsync(
                     chatId: message.Chat.Id,
                     text: $"<b>Укажите номер вашей заявки</b>",
                     cancellationToken: ct,
                     parseMode: ParseMode.Html,
                     replyMarkup: rkm);
                return;
            }

            if ((message.Text == "Вернуться к вопросам") || (message.Text == "Выбрать вопрос по работе в ЕЦП"))
            {
                this.InitQuestMenu();

                _memoryStorage.GetSession(message.Chat.Id).Operation = "no";
                _memoryStorage.GetSession(message.Chat.Id).NumApplication = "";
                _memoryStorage.GetSession(message.Chat.Id).Count = 0;

                this.rkm = new ReplyKeyboardMarkup(this.KeyboardButton.Select(quest => new[] { new KeyboardButton(quest) }).ToArray());
                this.rkm.ResizeKeyboard = true;
                await _telegramClient.SendTextMessageAsync(
                     chatId: message.Chat.Id,
                     text: message.Text,
                     cancellationToken: ct,
                     parseMode: ParseMode.Html,
                     replyMarkup: rkm);
                return;
            }            

            if ( message.Text == "Написать в техподдержку") 
            {
                _memoryStorage.GetSession(message.Chat.Id).Operation = "toemail"; 
                _memoryStorage.GetSession(message.Chat.Id).NumApplication = "";
                _memoryStorage.GetSession(message.Chat.Id).Count = 0;

                this.rkm = new(new[] { new KeyboardButton[] { "Отмена" } })
                { ResizeKeyboard = true };
                string s = "Тема :" + _memoryStorage.GetSession(message.Chat.Id).Quest + "\n\nОпишите свою проблему : Будет определена заявка\n\nТребования к оформлению обращения:\n\n" +
                           "В описании обращения должно быть указано пошаговое воспроизведение ошибки\n" +
                           "- условия воспроизведения (учетная запись (без пароля), версия операционной системы и браузера)," +
                           "- подробное описание последовательности действий, приводящих к ошибке.\n" +
                           "- при возможности должна быть предоставлена видеозапись с воспроизведением ошибки.\n" +
                           "- обращение должно содержать описание ожидаемого результата\n" +
                           "- снимки экрана должны быть полноэкранными, не допускаются снимки части экрана\n" +
                           "- рекомендуется использовать глаголы, передающие действия, например: предоставить, закрыть, исправить либо\n" +
                           "- существительные, передающие суть задачи для удобства поиска задачи в дальнейшем.";
                await _telegramClient.SendTextMessageAsync(
                         chatId: message.Chat.Id,
                         text: s,
                         cancellationToken: ct,
                         parseMode: ParseMode.Html,
                         replyMarkup: rkm);

                return;
            }

            if ( this.ListQuest.Contains( message.Text ) )
            {
                QuestionAsync( message, ct );
                return;
            }            

            switch ( message.Text )
            {

                case "/info1":

                    await _telegramClient.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       text: message.From.FirstName + " " + message.From.LastName + " " + message.From.Id,
                       replyMarkup: null,
                       cancellationToken: ct);

                    break;

                case "/kbd":

                    await _telegramClient.SendTextMessageAsync(
                       chatId: message.Chat.Id,
                       text: "Removing keyboard",
                       replyMarkup: new ReplyKeyboardRemove(),
                       cancellationToken: ct );                    

                    break;

                default:

                    string userOperation = _memoryStorage.GetSession( message.Chat.Id ).Operation;
                    if ( userOperation == "toemail" )
                    {
                        
                        string num_mess = System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 1000000).ToString();
                        _memoryStorage.GetSession(message.Chat.Id).NumApplication = num_mess;
                        string user_info = message.From.FirstName + " " + message.From.LastName + " " + message.From.Id;
                        string subject_mess = " Пользователь - " + user_info + ",\n Тема - " + _memoryStorage.GetSession(message.Chat.Id).Quest + ",\n Заявка номер - " + num_mess + "\n";
                        _memoryStorage.GetSession(message.Chat.Id).UserInfo = user_info;
                        _memoryStorage.GetSession(message.Chat.Id).SubjectMessage = subject_mess;

                        _operation.SendToEmail("html", message.Text, subject_mess, _appSettings.EmailFrom, _appSettings.EmailTo, _appSettings.ServSmtp, _appSettings.LoginEmail, _appSettings.PasswordEmail );
                                            
                        //await _telegramClient.SendTextMessageAsync( message.Chat.Id, "Ваша заявка зарегистрирована под номером " + num_mess + ". Пожалуйста сохраните номер вашей заявки для дальнейшей работы по ней.", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct );
                        //await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Ваша заявка зарегистрирована под номером " + num_mess + ". Пожалуйста сохраните номер вашей заявки для дальнейшей работы по ней.", replyMarkup: rkm, cancellationToken: ct);
                        // ==================
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, subject_mess + " Пожалуйста сохраните номер вашей заявки для дальнейшей\n работы по ней.\n\n" + " " + message.Text, replyMarkup: rkm, cancellationToken: ct);
                        await _telegramClient.SendTextMessageAsync(-4116444453, subject_mess + " Пожалуйста сохраните номер заявки для дальнейшей\n работы по ней.\n\n" + " " + message.Text, cancellationToken: ct);
                        
                        _memoryStorage.GetSession(message.Chat.Id).Operation = "no";
                        
                        NlogExtension.logger.Info("Send to email : mess=" + message.Text + ", subject=" + subject_mess + ", emailfrom=" + _appSettings.EmailFrom + ", emailto=" + _appSettings.EmailTo + ", servsmtp=" + _appSettings.ServSmtp, DateTime.Now);
                    
                    }
                    
                    if (userOperation == "worked")
                    {
                        _memoryStorage.GetSession(message.Chat.Id).Count = 0;
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Ваша заявка в работе", replyMarkup: rkm, cancellationToken: ct);
                        //await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Ваша заявка в работе", replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);
                       
                        //_memoryStorage.GetSession(message.Chat.Id).Operation = "no";
                        NlogExtension.logger.Info("worked - Ваша заявка в работе", DateTime.Now);                                                

                    }

                    break;

            }
			
        }
		
    }
	
}
