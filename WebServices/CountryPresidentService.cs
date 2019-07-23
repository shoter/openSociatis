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
    public class CountryPresidentService : ICountryPresidentService
    {
        private readonly ICountryRepository countryRepository;
        private readonly IPresidentVotingRepository presidentVotingRepository;
        private readonly IRegionRepository regionRepository;

        public CountryPresidentService(ICountryRepository countryRepository, IPresidentVotingRepository presidentVotingRepository, IRegionRepository regionRepository)
        {
            this.countryRepository = countryRepository;
            this.presidentVotingRepository = presidentVotingRepository;
            this.regionRepository = regionRepository;
        }

        public bool IsPresidentExcludingCountries(Citizen citizen, params int[] excludedCountriesIDs)
        {
            return countryRepository.Any
                (c => excludedCountriesIDs.Contains(c.ID) == false
                && c.PresidentID == citizen.ID);
        }

        /// <summary>
        /// Returns true if citizen is actually candidating in not finished votings
        /// </summary>
        public bool IsActuallyCandidating(Citizen citizen)
        {
            return presidentVotingRepository
                .NotFinishedVotings
                .Any(v => v.PresidentCandidates.Any(candidate => candidate.CandidateID == citizen.ID));
        }

        public MethodResult CanManageSpawn(Country country, Entity entity, Region region, bool value)
        {
            if (entity.Is(EntityTypeEnum.Citizen) == false)
                return new MethodResult("You must be a citizen to do that!");

            if (IsPresidentOf(entity.Citizen, country) == false)
                return new MethodResult("You are not a president!");

            if (region.CanSpawn && value == false && country.Regions.Count(r => r.CanSpawn) == 1)
                return new MethodResult("This is your last region where citizens can spawn. You cannot disable spawn here!");

            return MethodResult.Success;
        }

        public void ManageSpawn(Region region, bool state)
        {
            region.CanSpawn = state;
            regionRepository.SaveChanges();

        }

        public bool IsPresidentOf(Citizen citizen, Country country)
        {
            return country.PresidentID == citizen.ID;
        }

        public double GetGoldForCadency(int cadencyLength)
        {
            return Math.Round(((double)cadencyLength / (double)Constants.PresidentCadenceDefaultLength) * Constants.PresidentCadenceMedalGold, 2);
        }

    }
}
