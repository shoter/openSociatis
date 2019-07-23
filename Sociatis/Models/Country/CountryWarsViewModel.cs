using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebUtils;
using WebUtils.Extensions;

namespace Sociatis.Models.Country
{
    public class CountryWarsViewModel
    {
        public CountryInfoViewModel Info { get; set; }
        public PagingParam PagingParam { get; set; }

        public List<ShortWarInfoViewModel> WarsInfo { get; set; } = new List<ShortWarInfoViewModel>();





        public CountryWarsViewModel(IQueryable<Entities.War> wars, Entities.Country country, PagingParam pagingParam, IWarService warService)
        {
            Info = new CountryInfoViewModel(country);
            PagingParam = pagingParam;

            foreach(var war in wars.Apply(pagingParam).ToList())
            {
                WarsInfo.Add(new ShortWarInfoViewModel(war, warService));
                
            }

            

        }
    }
}