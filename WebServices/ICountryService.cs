using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.BigParams;
using WebServices.structs;

namespace WebServices
{
    public interface ICountryService
    {
        Country CreateCountry(CreateCountryParameters parameters);
        void ProcessPresidentVoting(int newDay, Country country, PresidentVoting presidentVoting);
        PresidentVoting CreateNewPresidentVoting(Entities.Country country, int votingDay);
        void ProcessDayChange(int newDay);
        void CandidateAsPresident(Citizen citizen, Country country);
        MethodResult CanCandidateAsPresident(Citizen citizen, Country country);

        MethodResult CanVoteInPresidentElections(Citizen citizen, PresidentVoting voting);
        MethodResult CanVoteOnPresidentCandidate(Citizen citizen, PresidentCandidate candidate);
        void VoteOnPresidentCandidate(Citizen citizen, PresidentCandidate candidate);

        bool IsPresident(Country country, Entity entity);
        MethodResult CanCreateCountryCompany(Entity entity, Country country, Citizen loggedCitizen);
        MethodResult CanCreateCountryCompany(string companyName, Entity entity, Country country, Citizen loggedCitizen, Region region, ProductTypeEnum productType);

    }
}
