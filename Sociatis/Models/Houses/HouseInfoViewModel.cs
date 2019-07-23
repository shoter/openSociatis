using Common.Drawings;
using Entities;
using Entities.enums;
using Sociatis.Helpers;
using Sociatis.Models.Infos;
using SociatisCommon.Rights;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis.Models.Houses
{
    public class HouseInfoViewModel
    {
        public class HouseFurnitureInfoViewModel
        {
            public string Name { get; set; }
            public int Quality { get; set; }

            public HouseFurnitureInfoViewModel(HouseFurniture furniture)
            {
                Name = ((FurnitureTypeEnum)furniture.FurnitureTypeID).ToString();
                Quality = furniture.Quality;
            }
        }
        public ImageViewModel Avatar { get; set; }
        public long HouseID { get; set; }
        public string OwnerName { get; set; }
        public string RegionName { get; set; }
        public int RegionID { get; set; }
        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public int ConditionPercent { get; set; }
        public double Condition { get; set; }
        public string ConditionColor { get; set; }
        public List<HouseFurnitureInfoViewModel> Furnitures { get; set; } = new List<HouseFurnitureInfoViewModel>();
        public InfoMenuViewModel Menu { get; set; }
        public HouseRights HouseRights { get; set; }

        public decimal? SellPrice;
        public string PriceSymbol { get; set; }

        public HouseInfoViewModel(House house, HouseRights rights)
        {
            HouseRights = rights;

            var region = house.Region;
            HouseID = house.ID;
            RegionName = region.Name;
            RegionID = region.ID;
            OwnerName = house.Citizen.Entity.Name;
            CountryName = Persistent.Countries.GetById(region.CountryID.Value).Entity.Name;
            CountryID = region.CountryID.Value;
            Avatar = Images.HousePlaceholder.VM;

            foreach (var f in house.HouseFurnitures.ToList())
            {
                Furnitures.Add(new HouseFurnitureInfoViewModel(f));
            }

            ConditionPercent = (int)house.Condition;
            Condition = (double)house.Condition;

            ConditionColor = ColorInterpolator
                .Lerp(
                Condition / 100.0,
                Color.Red,
                Color.Orange,
                Color.Green).ToHex();

            if (house.SellHouse != null)
            {
                SellPrice = house.SellHouse.Price;
                PriceSymbol = Persistent.Countries.GetCountryCurrency(house.Region.CountryID.Value).Symbol;
            }

            prepareMenu();
        }

        private void prepareMenu()
        {
            Menu = new InfoMenuViewModel();

            if (HouseRights.CanModifyHouse)
                Menu.AddItem(new InfoActionViewModel("BuyCM", "House", "Buy House Supplies", "fa-shopping-cart", new { houseID = HouseID }))
                    .AddItem(new InfoActionViewModel("Chest", "House", "Chest", "fa-cubes", new { houseID = HouseID }));

            Menu.AddItem(new InfoActionViewModel("Furniture", "House", "Furniture", "fa-bed", new { houseID = HouseID }));

            if(HouseRights.CanModifyHouse && SellPrice.HasValue == false)
                Menu.AddItem(new InfoActionViewModel("SellHouse", "House", "Sell House", "fa-usd", new { houseID = HouseID }));

            if(HouseRights.CanModifyHouse == false && SellPrice.HasValue)
                Menu.AddItem(new InfoActionViewModel("BuyHouse", "House", "Buy House", "fa-usd", System.Web.Mvc.FormMethod.Post, new { houseID = HouseID }));
        }

    }
}
