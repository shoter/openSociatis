using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis_Test_Suite.Dummies
{
    public class FriendDummyCreator : IDummyCreator<Friend>
    {
        static protected UniqueIDGenerator UniqueID = new UniqueIDGenerator();

        CitizenDummyCreator citizenCreator = new CitizenDummyCreator();

        Friend friend;

        public FriendDummyCreator()
        {
            friend = getNew();
        }

        Friend getNew()
        {
            Friend friend = new Friend();
            friend.ProposerCitizen = citizenCreator.Create();
            friend.SecondCitizen = citizenCreator.Create();
            friend.ProposerCitizenID = friend.ProposerCitizen.ID;
            friend.SecondCitizenID = friend.SecondCitizen.ID;
            return friend;
        }

        public Friend Create()
        {
            friend.ID = UniqueID;
            friend.DayCreated = GameHelper.CurrentDay;
            Friend temp = friend;
            friend = getNew();
            return temp;
            
        }
    }
}
