using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class NewDayRepository : RepositoryBase<NewDay, SociatisEntities>, INewDayRepository
    {
        public NewDayRepository(SociatisEntities context) : base(context)
        {
        }

        public void AddInformationAboutNewDay(int newDay)
        {
            Add(new NewDay()
            {
                DateTime = DateTime.Now,
                NewDay1 = newDay
            });
        }
    }
}
