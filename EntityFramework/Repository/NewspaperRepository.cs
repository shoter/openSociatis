using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.enums;
using Common.EntityFramework;

namespace Entities.Repository
{
    public class NewspaperRepository : RepositoryBase<Newspaper, SociatisEntities>, INewspaperRepository
    {
        public NewspaperRepository(SociatisEntities context) : base(context)
        {
        }

        public IQueryable<Newspaper> GetActionableNewspapers(int entityID)
        {
            return Where(n => n.OwnerID == entityID);
        }

        public void AddJournalist(NewspaperJournalist journalist)
        {
            context.NewspaperJournalists.Add(journalist);
        }

        public void RemoveJournalist(int journalistID, int newspaperID)
        {
            RemoveJournalist(GetJournalist(journalistID, newspaperID));
        }

        public void RemoveJournalist(NewspaperJournalist journalist)
        {
            context.NewspaperJournalists.Remove(journalist);
        }

        public NewspaperJournalist GetJournalist(int journalistID, int newspaperID)
        {
            return context.NewspaperJournalists.FirstOrDefault(j => j.CitizenID == journalistID && j.NewspaperID == newspaperID);
        }

        public List<NewspaperJournalist> GetJournalists(List<int> journalistIDs, int newspaperID)
        {
            return context.NewspaperJournalists
                .Where(j => journalistIDs.Contains(j.CitizenID) && j.NewspaperID == newspaperID)
                .ToList();
        }

        public void ChangeJournalistRights(int journalistID, int newspaperID, NewspaperRightsEnum rights)
        {
            var journalist = context.NewspaperJournalists.First(j => j.CitizenID == journalistID && j.NewspaperID == newspaperID);

            journalist.CanWriteArticles = rights.HasFlag(NewspaperRightsEnum.CanWriteArticles);
            journalist.CanManageArticles = rights.HasFlag(NewspaperRightsEnum.CanManageArticles);
            journalist.CanManageJournalists = rights.HasFlag(NewspaperRightsEnum.CanManageJournalists);
        }
    }
}
