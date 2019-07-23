using Entities.Models.Constructions;
using Sociatis.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Constructions
{
    public class NationalConstructionsViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public List<NationalConstructionItemViewModel> Constructions { get; set; } = new List<NationalConstructionItemViewModel>();

        public NationalConstructionsViewModel(Entities.Country country, IEnumerable<NationalConstruction> constructions, IConstructionService constructionService)
        {
            Info = new CountryInfoViewModel(country);
            Constructions.AddRange(constructions.Select(c => new NationalConstructionItemViewModel(c, constructionService)));
        }
    }
}
