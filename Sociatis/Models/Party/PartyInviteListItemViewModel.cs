using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Party
{
    public class PartyInviteListItemViewModel
    {
        public int InviteID { get; set; }
        public int PartyID { get; set; }
        public string PartyName { get; set; }
        public SmallEntityAvatarViewModel PartyAvatar { get; set; }

        public PartyInviteListItemViewModel(PartyInvite invite)
        {
            InviteID = invite.ID;
            PartyID = invite.PartyID;
            PartyName = invite.Party.Entity.Name;
            PartyAvatar = new SmallEntityAvatarViewModel(invite.Party.Entity)
                .DisableNameInclude();
        }

    }
}