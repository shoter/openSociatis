using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Country
{
    public class DeclareEmbargoViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public List<SelectListItem> PossiblEmbargoes { get; set; } = new List<SelectListItem>();
        public int CountryEmbargoID { get; set; }

        public DeclareEmbargoViewModel() { }
        public DeclareEmbargoViewModel(Entities.Country country, List<Entity> possibleCountries)
        {
            Info = new CountryInfoViewModel(country);


            PossiblEmbargoes.Add(new SelectListItem()
            {
                Value = "null",
                Text = "-- Select --"
            });

            foreach(var possibleCountry in possibleCountries)
            {
                PossiblEmbargoes.Add(new SelectListItem()
                {
                    Value = possibleCountry.EntityID.ToString(),
                    Text = possibleCountry.Name
                });
            }


        }
    }
}