using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INF2course.Controllers
{
    internal class SessionManager
    {
        static Dictionary<Guid, Session> MemoryCache = new Dictionary<Guid, Session>();

        public static Session Create(int accountId, string email)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Email = email,
                CreateDateTime = DateTime.Now
            };
            MemoryCache.Add(session.Id, session);
            return session;
        }

        public static bool IsValid(Guid sessionId)
        {
            if (!MemoryCache.ContainsKey(sessionId))
            {
                return false;
            }
            Session session = MemoryCache[sessionId];
            bool result = session.CreateDateTime.AddMinutes(2) > DateTime.Now;
            if (result == false)
            {
                MemoryCache.Remove(sessionId);
            }

            return result;
        }

        public static Session GetById(Guid sessionId)
        {
            if (!MemoryCache.ContainsKey(sessionId))
            {
                return null;
            }
            Session session = MemoryCache[sessionId];
            bool result = session.CreateDateTime.AddMinutes(2) > DateTime.Now;
            if (result == false)
            {
                MemoryCache.Remove(sessionId);
                return null;
            }

            return session;
        } 
    }
}
