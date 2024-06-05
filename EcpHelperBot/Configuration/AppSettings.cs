using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcpHelperBot.Configuration
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string BotToken { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string ServSmtp { get; set; }
        public string LoginEmail { get; set; }
        public string PasswordEmail { get; set; }
        public string QuestAnsw { get; set; }
        public string DownloadsFolder { get; set; }
        public string ImageFileName { get; set; }

    }

}
