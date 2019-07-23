using Common.EntityFramework;
using Entities.enums;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface INewspaperRepository : IRepository<Newspaper>
    {
        IQueryable<Newspaper> GetActionableNewspapers(int entityID);
        void AddJournalist(NewspaperJournalist journalist);
        List<NewspaperJournalist> GetJournalists(List<int> journalistIDs, int newspaperID);

        void ChangeJournalistRights(int journalistID, int newspaperID, NewspaperRightsEnum rights);
        NewspaperJournalist GetJournalist(int journalistID, int newspaperID);
        void RemoveJournalist(int journalistID, int newspaperID);
        void RemoveJournalist(NewspaperJournalist journalist);

    }
}
