using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Country
{
    public class EmbargoesPageViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public List<EmbargoViewModel> CreatedEmbargoes { get; set; } = new List<EmbargoViewModel>();
        public List<EmbargoViewModel> Embargoes { get; set; } = new List<EmbargoViewModel>();

        public EmbargoesPageViewModel(Entities.Country country, IEmbargoService embargoService)
        {
            Info = new CountryInfoViewModel(country);
            foreach(var embargo in country.CreatedEmbargoes.Where(e => e.Active).ToList())
            {
                CreatedEmbargoes.Add(new EmbargoViewModel(embargo, embargoService));
            }

            foreach (var embargo in country.Embargoes.Where(e => e.Active).ToList())
            {
                Embargoes.Add(new EmbargoViewModel(embargo, embargoService));
            }
        }
    }
}