using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Entities
{
    public static class CountryExtensions
    {
        public static void SetPresident(this Country country, Citizen citizen)
        {
            country.President = citizen;
            country.PresidentID = citizen.ID;
            citizen.CountriesPresident.Add(country);
        }
    }
}
