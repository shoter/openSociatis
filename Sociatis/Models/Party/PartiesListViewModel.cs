using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUtils;

namespace Sociatis.Models.Party
{
    public class PartiesListViewModel
    {
        public List<PartyInfoViewModel> Infos { get; set; } = new List<PartyInfoViewModel>();
        public PagingParam PagingParam { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public PartiesListViewModel(Entities.Country country, List<Entities.Party> parties, PagingParam pagingParam)
        {
            CountryID = country.ID;
            CountryName = country.Entity.Name;
            PagingParam = pagingParam;
            foreach(var party in parties)
            {
                Infos.Add(new PartyInfoViewModel(party));
            }
        }
    }
}