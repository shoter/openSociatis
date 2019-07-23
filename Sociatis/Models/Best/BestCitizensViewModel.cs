using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Best
{
    public class BestCitizensViewModel
    {
        public Dictionary<int, string> Countries { get; set; } = new Dictionary<int, string>();

        public BestCitizensViewModel()
        {
            foreach (var country in Persistent.Countries.GetAll())
                Countries.Add(country.ID, country.Entity.Name);
        }
    }
}