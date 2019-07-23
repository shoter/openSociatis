using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Country
{
    public class DeclareWarViewModel : WarDeclarationViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public List<SelectListItem> NeighboursCountries { get; set; } = new List<SelectListItem>();


        public DeclareWarViewModel(Entities.Country country, ICountryRepository countryRepository)
        {
            Info = new CountryInfoViewModel(country);
            AttackerCountryID = country.ID;

            var neighbours = getNeighboursWithoutWarWithAttacker(country, countryRepository)
                .Select(n => new { ID = n.ID, Name = n.Entity.Name })
                .ToList();

            NeighboursCountries.Add(new SelectListItem() { Text = "Select country", Value = "" });

            var allies = countryRepository.GetAllies(country.ID);

            foreach (var neighbour in neighbours)
            {
                if (allies.Any(a => a.ID == neighbour.ID))
                    continue;

                NeighboursCountries.Add(new SelectListItem() { Text = neighbour.Name, Value = neighbour.ID.ToString() });
            }
        }

        private IQueryable<Entities.Country> getNeighboursWithoutWarWithAttacker(Entities.Country country, ICountryRepository countryRepository)
        {
            return countryRepository.GetNeighbourCountries(country.ID)
                            .Where(n => n.AttackerWars.Any(w => w.Active && (w.AttackerCountryID == AttackerCountryID 
                                                                  || w.DefenderCountryID == AttackerCountryID)) == false)
                            .Where(n => n.DefenderWars.Any(w => w.Active && (w.AttackerCountryID == AttackerCountryID
                                                                  || w.DefenderCountryID == AttackerCountryID)) == false);
        }
    }
}