using Entities;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Regions
{
    public class RegionInfoViewModel
    {
        public string RegionName { get; set; }
        public int RegionID { get; set; }
        public string CountryName { get; set; }
        public int? CountryID { get; set; }
        public int Population { get; set; }
        public int? CoreCountryID { get; set; }
        public string CoreCountryName { get; set; }
        public ImageViewModel CountryFlag { get; set; }

        public bool CanStartRessistanceWar { get; set; } = false;
        public MoneyViewModel RessistanceCost { get; set; } = null;

        public RegionInfoViewModel(Region region)
        {
            RegionName = region.Name;
            RegionID = region.ID;
            CountryName = region.Country.Entity.Name;
            CountryID = region.CountryID;
            CountryFlag = Images.GetCountryFlag(CountryName).VM;
            Population = region.Citizens.Count;

            var currentEntity = SessionHelper.CurrentEntity;
            var warService = DependencyResolver.Current.GetService<IWarService>();

            

            CoreCountryID = region.CountryCoreID;
            if (CoreCountryID.HasValue)
            {
                CoreCountryName = Persistent.Countries.GetById(CoreCountryID.Value).Entity.Name;

                CanStartRessistanceWar = warService.CanStartRessistanceBattle(currentEntity, region).isSuccess;
                if (CanStartRessistanceWar)
                    RessistanceCost = new MoneyViewModel(warService.GetMoneyNeededToStartResistanceBattle(region));
            }
        }
    }
}