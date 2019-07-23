using Entities;
using Sociatis.Models.Citizens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Friends
{
    public class FriendsViewModel
    {
        public CitizenInfoViewModel Info { get; set; }
        public List<ViewCitizenFriendModel> Friends { get; set; } = new List<ViewCitizenFriendModel>();

        public FriendsViewModel(Citizen citizen, IQueryable<Citizen> friends, IFriendService friendService)
        {
            Info = new CitizenInfoViewModel(citizen, friendService);
            Friends = friends
                .OrderBy(f => f.Entity.Name)
                .ToList() //to make SQL query
                .Select(f => new ViewCitizenFriendModel(f))
                .ToList();
        }

    }
}