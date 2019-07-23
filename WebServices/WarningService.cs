using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weber.Html;
using WebServices.Helpers;

namespace WebServices
{
    public class WarningService : BaseService, IWarningService
    {
        private readonly IWarningRepository warningRepository;
        private readonly IEntityRepository entityRepository;
        private readonly ICongressmenRepository congressmenRepository;
        public WarningService(IWarningRepository warningRepository, IEntityRepository entityRepository, ICongressmenRepository congressmenRepository)
        {
            this.warningRepository = warningRepository;
            this.entityRepository = entityRepository;
            this.congressmenRepository = congressmenRepository;
        }

        public void SendWarningToCongress(Country country, string message)
        {
            var congressman = congressmenRepository.Where(c => c.CountryID == country.ID).Select(c => c.CitizenID).ToList();
            var countryLink = EntityLinkCreator.Create(country.Entity);
            message = $"[Redirected from {countryLink}'s congress] {message}";
            foreach (var congressmanID in congressman)
            {
                AddWarning(congressmanID, message);
            }
        }

        public void AddWarning(int entityID, string message)
        {
            var warning = new Warning()
            {
                DateTime = DateTime.Now,
                Day = GameHelper.CurrentDay,
                EntityID = entityID,
                Unread = true,
                Message = message
            };
            
            using (NoSaveChanges)
            {
                var entity = entityRepository.GetById(entityID);
                if (entity.GetEntityType() == EntityTypeEnum.Country)
                {
                    var countryLink = EntityLinkCreator.Create(entity);
                    var presidentMessage = $"[Redirected from {countryLink}] {message}";
                    if (entity.Country.PresidentID.HasValue)
                        AddWarning(entity.Country.PresidentID.Value, presidentMessage);
                }
                else if (entity.GetEntityType() == EntityTypeEnum.Party)
                {
                    var president = entity.Party.GetPresident();
                    if (president != null)
                    {
                        var partyLink = EntityLinkCreator.Create(entity);
                        var presidentMessage = $"[Redirected from {partyLink}] {message}";
                        AddWarning(president.CitizenID, presidentMessage);
                    }
                }
            }
            warningRepository.Add(warning);
            ConditionalSaveChanges(warningRepository);
        }
    }
}
