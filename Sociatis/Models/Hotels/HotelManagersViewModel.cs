using Entities.Models.Hotels;
using Sociatis.Controllers;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Hotels
{
    public class HotelManagersViewModel
    {
        public class HotelManagerViewModel
        {
            public int HotelID { get; set; }
            public bool ReadOnly { get; set; }
            public int CitizenID { get; set; }
            public string Title { get; set; }
            public SmallEntityAvatarViewModel Avatar { get; set; }
            public HotelRights Rights { get; set; }

            public HotelManagerViewModel(int hotelID, HotelManagerModel manager, HotelRights rights, string title = "Manager")
            {
                HotelID = hotelID;
                CitizenID = manager.ManagerID;
                Avatar = new SmallEntityAvatarViewModel(manager.ManagerID, manager.ManagerName, manager.ImgURL);
                Rights = manager.HotelRights;
                ReadOnly = 
                    rights.CanManageManagers == false || rights.Priority <= manager.HotelRights.Priority;
                Title = title;
            }

            public HotelManagerViewModel() { }
        }
        public HotelInfoViewModel Info { get; set; }
        public HotelRights Rights => Info.HotelRights;
        public List<HotelManagerViewModel> Managers { get; set; } = new List<HotelManagerViewModel>();
        public Select2AjaxViewModel ManagerSelector { get; set; }

        public HotelManagersViewModel(HotelInfo info, IEnumerable<HotelManagerModel> managers)
        {
            Info = new HotelInfoViewModel(info);

            Managers.Add(new HotelManagerViewModel()
            {
                Avatar = new SmallEntityAvatarViewModel(info.OwnerID, info.OwnerName, info.OwnerImgUrl),
                CitizenID = info.OwnerID,
                ReadOnly = true,
                Rights = HotelRights.FullRights,
                Title = "Owner",
                HotelID = info.HotelID
            });
            Managers.AddRange(managers.Select(m => new HotelManagerViewModel(info.HotelID, m, Rights)).OrderByDescending(m => m.Rights.Priority).ToList());
            ManagerSelector = Select2AjaxViewModel.Create<CitizenController>(x => x.GetCitizens(null),
                "citizenID", null, "");
        }
    }
}
