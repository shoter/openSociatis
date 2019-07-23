using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress
{
    public class CongressMembersListViewModel
    {
        public List<CongressMemberViewModel> Members { get; set; } = new List<CongressMemberViewModel>();
        public string PartyName { get; set; }
        public CongressMembersListViewModel(List<Entities.Congressman> congressmen, Entities.Party party)
        {
            PartyName = party?.Entity.Name ?? "No party";

            foreach (var congressman in congressmen)
            {
                Members.Add(new CongressMemberViewModel(congressman));
            }
        }
    }
}