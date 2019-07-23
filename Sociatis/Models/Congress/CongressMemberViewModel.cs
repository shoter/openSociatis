using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress
{
    public class CongressMemberViewModel
    {
        public string CitizenName { get; set; }
        public string PartyName { get; set; }
        public ImageViewModel Avatar { get; set; }

        public CongressMemberViewModel(Entities.Congressman congressman)
        {
            var citizen = congressman.Citizen;
            CitizenName = citizen.Entity.Name;

            if (citizen.PartyMember == null)
                PartyName = "N\\A";
            else
                PartyName = citizen?.PartyMember?.Party?.Entity?.Name ?? "No party";

            Avatar = new ImageViewModel(citizen.Entity.ImgUrl);
        }
    }
}