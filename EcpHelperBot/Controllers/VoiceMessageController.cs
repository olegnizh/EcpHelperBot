using Telegram.Bot;
using Telegram.Bot.Types;

namespace EcpHelperBot.Controllers
{

    public class VoiceMessageController
    {        
        private readonly ITelegramBotClient _telegramClient;

        public VoiceMessageController(ITelegramBotClient telegramBotClient)
        {            
            _telegramClient = telegramBotClient;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            var fileId = message.Voice?.FileId;
            if (fileId == null)
                return;

            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Получено звуковое сообщение", cancellationToken: ct);

        }
		
    }
	
}