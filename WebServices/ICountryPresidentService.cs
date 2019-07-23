using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ICountryPresidentService
    {
        bool IsPresidentExcludingCountries(Citizen citizen, params int[] excludedCountriesIDs);
        /// <summary>
        /// Returns true if citizen is actually candidating in not finished votings
        /// </summary>
        bool IsActuallyCandidating(Citizen citizen);

        MethodResult CanManageSpawn(Country country, Entity entity, Region region, bool value);
        void ManageSpawn(Region region, bool state);
        double GetGoldForCadency(int cadencyLength);
    }
}
