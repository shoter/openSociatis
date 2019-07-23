using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace Sociatis.Models.Party
{
    public class PartySummaryViewModel : BaseEntitySummaryViewModel
    {
        public ImageViewModel Avatar { get; set; }
        public int PartyID { get; set; }
        public string Name { get; set; }
        public PartySummaryViewModel(Entities.Party party, Session session) : base(session)
        {
            Avatar = new ImageViewModel(party.Entity.ImgUrl);
            Name = party.Entity.Name;
            PartyID = party.ID;
        }
    }
}