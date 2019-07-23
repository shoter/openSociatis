using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Country
{
    public class ShortWarCountryInfoViewModel
    {
        public string Name { get; set; }
        public ImageViewModel Flag { get; set; }
        public int AllyCount { get; set; }

        public ShortWarCountryInfoViewModel() { }
        public ShortWarCountryInfoViewModel(Entities.Country country)
        {
            Name = country.Entity.Name; ;
            Flag = Images.GetCountryFlag(Name).VM;
        }
    }
}