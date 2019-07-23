using Common.Extensions;
using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Models.Houses
{
    public class HouseIndexViewModel
    {
        public class HouseViewModel
        {
            public class FurnitureViewModel
            {
                public string Name { get; set; }
                public int Quality { get; set; }
                public FurnitureViewModel(HouseFurniture f)
                {
                    Name = ((FurnitureTypeEnum)f.FurnitureTypeID).ToHumanReadable().FirstUpper();
                    Quality = f.Quality;
                }
            }
            public long HouseID { get; set; }
            public ImageViewModel Image { get; set; }
            public string RegionName { get; set; }
            public int RegionID { get; set; }
            public string CountryName { get; set; }
            public int CountryID { get; set; }
            public string Condition { get; set; }

            public List<FurnitureViewModel> Furniture { get; set; }

            public bool IsInside { get; set; }

            public HouseViewModel(House house)
            {
                HouseID = house.ID;
                Image = Images.HousePlaceholder.VM;
                RegionName = house.Region.Name;
                RegionID = house.Region.ID;
                CountryID = house.Region.CountryID.Value;
                CountryName = house.Region.Country.Entity.Name;
                Condition = Math.Round(house.Condition, 1).ToString();

                Furniture = house.HouseFurnitures.ToList()
                    .Select(f => new FurnitureViewModel(f))
                    .ToList();

                if (SessionHelper.CurrentEntity.EntityID == SessionHelper.LoggedCitizen.ID)
                {
                    if (house.RegionID == SessionHelper.LoggedCitizen.RegionID)
                        IsInside = true;
                }


            }
        }

        public List<HouseViewModel> Houses { get; set; }

        public HouseIndexViewModel(IEnumerable<House> houses)
        {
            Houses = houses.Select(h => new HouseViewModel(h)).ToList();
        }
    }
}
