using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class WarningRepository : RepositoryBase<Warning, SociatisEntities>, IWarningRepository
    {
        public WarningRepository(SociatisEntities context) : base(context)
        {
        }

        public void ReadWarnings(List<Warning> warnings)
        {
            using (var db = new SociatisEntities())
            {
                foreach (var warning in warnings)
                {
                    var entity = new Warning();
                    entity.ID = warning.ID;
                    entity.Unread = false;
                    db.Set<Warning>().Attach(entity);
                    db.Entry(entity).Property(w => w.Unread).IsModified = true;

                }
                db.SaveChanges();
            }
        }
    }
}
