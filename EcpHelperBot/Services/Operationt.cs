using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using MailKit.Security;
using MimeKit.Utils;
using Telegram.Bot.Types;


namespace EcpHelperBot.Services
{

    public class Operation : IOperation
    {       

        public void SendToEmail( string format, string message, string subject, string from, string to, string servsmtp, string login, string password )
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse( from ));
            email.To.Add(MailboxAddress.Parse( to ));
            email.Subject = subject;
            if (format == "text")
                email.Body = new TextPart(TextFormat.Text) { Text = message };
            if (format == "html")
                email.Body = new TextPart( TextFormat.Html ) { Text = message };

            using var smtp = new SmtpClient();

            smtp.Connect( servsmtp, 587 );
            smtp.Authenticate( login, password );
            smtp.Send( email );
            smtp.Disconnect( true );

        }

        public string Cnt( string message )
        {
            if ( message != null )
            {
                return string.Concat( message.Length.ToString(), " ", Pref( message.Length ) );
            }
            else
                return "";

        }


        public string Sum( string message )
        {
            try
            {
                if ( message != null )
                {
                    var mes = message.Split(' ');
                    int sum = 0;
                    foreach ( var word in mes )
                    {
                        sum += int.Parse( word );
                    }
                    return string.Concat( "Сумма чисел в сообщении: ", sum );
                }
                else
                    return "";
            }
            catch ( Exception e )
            {
                if ( e.Message == "Value was either too large or too small for an Int32." )
                    return "слишком большое число";
                if ( e.Message == "Input string was not in a correct format." )
                    return "Вещественные числа не поддерживаются";
                return "Неверный формат, напишите числа через пробел";
            }
        }


        private string Pref(int n)
        {
            if (n <= 20)
            {
                return n switch
                {
                    1 => "символ",
                    3 or 4 or 2 => "символа",
                    _ => "символов",
                };
            }
            else
            {
                return (n % 10) switch
                {
                    1 => "символ",
                    3 or 4 or 2 => "символа",
                    _ => "символов",
                };
            }

        }

    }

}
