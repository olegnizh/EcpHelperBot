using System.Collections.Concurrent;
using EcpHelperBot.Models;
using EcpHelperBot.Services;


namespace EcpHelperBot.Services
{
    public class MemoryStorage : IStorage
    {
        private readonly ConcurrentDictionary<long, Session> _sessions;

        public MemoryStorage()
        {
            _sessions = new ConcurrentDictionary<long, Session>();
        }

        public Session GetSession(long chatId)
        {
            // Возвращаем сессию по ключу, если она существует
            if (_sessions.ContainsKey(chatId))
                return _sessions[chatId];

            // Создаем и возвращаем новую, если такой не было
            var newSession = new Session() { Operation = "no", Flag = 0, Quest = "", Count = 0, SubjectMessage = "", NumApplication = "" };
            _sessions.TryAdd(chatId, newSession);
            return newSession;
        }

    }

}
