using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class SessionRepository : RepositoryBase<Session, SociatisEntities>, ISessionRepository
    {
        public SessionRepository(SociatisEntities context) : base(context)
        {}

        public void UpdateSession(Session session, DateTime expireTime)
        {
            UpdateSingleField(x => x.ExpirationDate, s => { s.ID = session.ID; s.IP = session.IP; s.Cookie = session.Cookie; }, expireTime);
        }
    }
}
