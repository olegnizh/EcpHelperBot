using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace EcpHelperBot.Services
{
    public interface IFileHandler
    {
        Task Download(string fileId, CancellationToken ct, string imageid);
        Task SendToEmailImage(string imageid, string subject, string from, string to, string servsmtp, string login, string password);

    }

}
