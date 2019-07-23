using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Friends
{
    public class ViewCitizenFriendModel : SmallEntityAvatarViewModel
    {
        public ViewCitizenFriendModel(Citizen friend) : base(friend.Entity)
        {
            Classname = "relative";
        }
    }
}