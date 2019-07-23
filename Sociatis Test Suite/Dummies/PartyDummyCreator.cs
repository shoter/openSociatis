using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class PartyDummyCreator : EntityDummyCreator, IDummyCreator<Party>
    {
        private Party party
        {
            get { return entity.Party; }
            set
            {
                value.Entity = entity;
                value.ID = entity.EntityID;
                entity.Party = value;
            }
        }
    
        public PartyDummyCreator()
        {
            createParty();
        }

        public new Party Create()
        {
            var _return = party;
            createParty();
            return _return;
        }

        public void AddMember(Citizen citizen, PartyRoleEnum partyRole)
        {
            PartyMember member = new PartyMember()
            {
                Citizen = citizen,
                CitizenID = citizen.ID,
                Party = party,
                PartyID = party.ID,
                PartyRoleID = (int)partyRole
            };

            party.PartyMembers.Add(member);

            citizen.PartyMember = member;
        }

        private void createParty()
        {
            base.createEntity(EntityTypeEnum.Party);
            party = new Party();

        }

        internal void AddMember(Citizen ourCitizen, object partyRoleENum)
        {
            throw new NotImplementedException();
        }
    }
}