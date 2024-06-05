namespace EcpHelperBot.Services
{
    public interface IOperation
    {
        
        void SendToEmail(string format, string message, string subject, string from, string to, string servsmtp, string login, string password );
        string Sum( string message );
        string Cnt( string message );

    }

}
