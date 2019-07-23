using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class FriendRequestDummyCreator : IDummyCreator<FriendRequest>
    {
        static protected UniqueIDGenerator UniqueID = new UniqueIDGenerator();

        CitizenDummyCreator citizenCreator = new CitizenDummyCreator();

        FriendRequest friendRequest;

        public FriendRequestDummyCreator()
        {
            friendRequest = getNew();
        }

        FriendRequest getNew()
        {
            FriendRequest friend = new FriendRequest();
            friend.ProposerCitizen = citizenCreator.Create();
            friend.SecondCitizen = citizenCreator.Create();
            friend.ProposerCitizenID = friend.ProposerCitizen.ID;
            friend.SecondCitizenID = friend.SecondCitizen.ID;
            return friend;
        }

        public FriendRequest Create()
        {
            friendRequest.ID = UniqueID;
            FriendRequest temp = friendRequest;
            friendRequest = getNew();
            return temp;

        }
    }
}
