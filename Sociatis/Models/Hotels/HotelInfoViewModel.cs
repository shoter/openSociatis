using Common.Drawings;
using Common.Language;
using Entities.Models.Hotels;
using Sociatis.Models.Avatar;
using Sociatis.Models.Infos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sociatis.Models.Hotels
{
    public class HotelInfoViewModel
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public ImageViewModel Avatar { get; set; }

        public int ConditionPercent { get; set; }
        public double Condition { get; set; }
        public string RoomsDescription { get; set; }

        public HotelRights HotelRights { get; set; }

        public InfoMenuViewModel Menu { get; set; }

        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public int OwnerID { get; set; }
        public string OwnerName { get; set; }

        public string ConditionColor { get; set; }

        public AvatarChangeViewModel AvatarChange { get; set; }

        public HotelInfoViewModel(HotelInfo info)
        {
            Name = info.HotelName;
            ID = info.HotelID;
            Avatar = new ImageViewModel(info.ImgUrl);
            ConditionPercent = (int)info.Condition;
            Condition = (double)info.Condition;
            RoomsDescription = prepareRoomDescription(info.HotelRoomInfos);
            HotelRights = info.HotelRights;
            RegionID = info.RegionID;
            RegionName = info.RegionName;
            CountryID = info.CountryID;
            CountryName = info.CountryName;
            OwnerID = info.OwnerID;
            OwnerName = info.OwnerName;


            Menu = prepareMenu();

            ConditionColor = ColorInterpolator
                .Lerp(
                Condition / 100.0,
                Color.Red,
                Color.Orange,
                Color.Green).ToHex();

            if (HotelRights.AnyRights)
            {
                AvatarChange = new AvatarChangeViewModel(ID);
            }

        }

        private InfoMenuViewModel prepareMenu()
        {
            Menu = new InfoMenuViewModel();

            Menu.AddItem(InfoExpandableViewModel.CreateExchange(ID));

            var info = new InfoExpandableViewModel("Info", "fa-question");

            info.AddChild(new InfoActionViewModel("Rooms", "Hotel", "Rooms", "fa-bed", new { HotelID = ID }));
            info.AddChild(new InfoActionViewModel("Managers", "Hotel", "Managers", "fa-users", new { HotelID = ID }));

            Menu.AddItem(info);

            if (HotelRights.AnyRights)
            {
                var manage = new InfoExpandableViewModel("Manage", "fa-cogs");

                if (HotelRights.CanBuildRooms)
                    manage.AddChild(new InfoActionViewModel("CreateRoom", "Hotel", "Build room", "fa-bed", new { HotelID = ID }));
                if (HotelRights.CanMakeDeliveries)
                    manage.AddChild(new InfoActionViewModel("MakeDelivery", "Hotel", "Create delivery", "fa-truck", new { HotelID = ID }));
                if (HotelRights.CanSetPrices)
                    manage.AddChild(new InfoActionViewModel("SetPrices", "Hotel", "Pricing", "fa-usd", new { HotelID = ID }));
                if (HotelRights.CanUseWallet)
                    manage.AddChild(new InfoActionViewModel("Wallet", "Hotel", "Wallet", "fa-money", new { HotelID = ID }));
                if(HotelRights.CanManageManagers)
                    manage.AddChild(new InfoActionViewModel("Managers", "Hotel", "Managers", "fa-users", new { HotelID = ID }));
                if (HotelRights.CanManageEquipment)
                    manage.AddChild(new InfoActionViewModel("Inventory", "Hotel", "Inventory", "fa-cubes", new { HotelID = ID }));
                if (HotelRights.CanSwitchInto)
                    manage.AddChild(InfoActionViewModel.CreateEntitySwitch(ID));

                Menu.AddItem(manage);
            }

            return Menu;
        }

        private string prepareRoomDescription(IEnumerable<HotelRoomInfo> infos)
        {
            return string.Join(", ",
                infos.OrderBy(i => i.Quality)
                .Select(i => $"{i.Count} room{PluralHelper.S(i.Count)} Q{i.Quality}"));
        }
    }
}
