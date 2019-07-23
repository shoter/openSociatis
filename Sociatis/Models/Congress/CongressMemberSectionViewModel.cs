using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Congress
{
    public class CongressMemberSectionViewModel 
    {
        public List<CongressMembersListViewModel> OrderedMembersByParty { get; set; } = new List<CongressMembersListViewModel>();
        public CongressInfoViewModel Info { get; set; }
        public CongressMemberSectionViewModel(List<Entities.Congressman> unorderedCongressmen, Entities.Country country, IPartyRepository partyRepository)
        {
            Info = new CongressInfoViewModel(country);

            var congressmenOrderedByParty = unorderedCongressmen.GroupBy(c => c.Citizen?.PartyMember?.PartyID)
                .OrderByDescending(x => x.Count());

            var partysIDs = congressmenOrderedByParty.Select(c => c.Key).ToList();

            var parties = partyRepository.Where(p => partysIDs.Contains(p.ID)).ToList();

            foreach(var congressmen in congressmenOrderedByParty)
            {
                var party = parties.FirstOrDefault(p => p.ID == congressmen.Key);

                OrderedMembersByParty.Add(new CongressMembersListViewModel(congressmen.ToList(), party));
            }
        }
    }
}