using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public class CountryTreasureService : ICountryTreasureService
    {
        private readonly ICountryRepository countryRepository;

        public CountryTreasureService(ICountryRepository countryRepository)
        {
            this.countryRepository = countryRepository;
        }

        public MethodResult CanSeeCountryTreasure(Country country, Entity entity)
        {
            if (country == null)
                return new MethodResult("Country does not exist!");

            if (entity == null)
                throw new Exception("Entity does not exist");

            if (entity.GetEntityType() != EntityTypeEnum.Citizen)
                return new MethodResult("You are not a citizen to do that!");

            var treasureSetting = (LawAllowHolderEnum) countryRepository.GetCountryPolicySetting(country.ID, policy => policy.TreasuryVisibilityLawAllowHolderID);
            var citizen = entity.Citizen;



            if (treasureSetting != LawAllowHolderEnum.President)
            {
                if (citizen?.Congressmen.Any(c => c.CountryID == country.ID) == true)
                    return MethodResult.Success;
            }

            if (treasureSetting != LawAllowHolderEnum.Congress)
            {
                if (country.PresidentID == citizen.ID)
                    return MethodResult.Success;
            }

            return new MethodResult("You are not allowed to do that!");
        }
    }
}
