using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface INewDayRepository : IRepository<NewDay>
    {
        void AddInformationAboutNewDay(int newDay);
    }
}
