using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcpHelperBot.Models
{
    public class Session
    {
        public string Operation { get; set; }
        public int Flag { get; set; }
        public string Quest { get; set; }
        public int Count { get; set; }
        public string UserInfo { get; set; }
        public string SubjectMessage { get; set; }
        public string NumApplication { get; set; }

    }
}
