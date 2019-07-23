using Entities;
using Entities.Repository;
using Sociatis.Models.Country;
using Sociatis.Models.Hospitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Regions
{
    public class RegionViewModel
    {
        public RegionInfoViewModel Info { get; set; }
        public List<RegionResourceViewModel> Resources { get; set; } = new List<RegionResourceViewModel>();

        public HospitalHealViewModel Heal { get; set; }



        public RegionViewModel(Region region, IHospitalService hospitalService, IHospitalRepository hospitalRepository)
        {
             Info = new RegionInfoViewModel(region);

            var resources = region.Resources.ToList();

            foreach (var resource in resources)
                Resources.Add(new RegionResourceViewModel(resource));
            if (region.Hospital != null)
                Heal = new HospitalHealViewModel(region.Hospital, hospitalService, hospitalRepository, showHospitalName: true);
        }



    }
}