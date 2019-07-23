using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyJoinRequestsListItemViewModel
    {
        public int ID { get; set; }
        public SmallEntityAvatarViewModel Avatar { get; set; }
        public string Message { get; set; }
        public int RequestDay { get; set; }
        public DateTime RequestTime { get; set; }

        public PartyJoinRequestsListItemViewModel(PartyJoinRequest request)
        {
            ID = request.ID;
            Avatar = new SmallEntityAvatarViewModel(request.Citizen.Entity);
            Message = request.Message;
            RequestDay = request.Day;
            RequestTime = request.DateTime;
        }
    }
}