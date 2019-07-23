using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IPartyService
    {
        Party CreateParty(string Name, Citizen foundator, Country originCountry);
        PartyMember JoinParty(Citizen citizen, Party party);
        void ResignFromParty(Citizen citizen);
        void ResignFromParty(int citizenID);
        void KickFromParty(int kickerCitizenID, int kickedCitizenID);
        void DeleteAllPartyJoinRequests(Citizen citizen);
        void ProcessDayChange(int newDay);
        string CanCandidate(Party party, Citizen citizen);
        void Candidate(Party party, PartyMember member);
        void VoteForPresident(PartyMember member, PartyPresidentCandidate candidate);
        bool CanCandidateToCongress(Citizen citizen, Party party);
        bool DoesCitizenBelongToParty(Citizen citizen, Party party);
        CongressCandidate CandidateToCongress(Citizen citizen);
        bool CanAcceptCongressCandidates(Citizen citizen, Party party);
        void AcceptCongressCandidate(CongressCandidate candidate);

        MethodResult CanInviteCitizen(Citizen invitingCitizen, Party party, Citizen invitedCitizen);
        void InviteCitizen(Citizen citizen, Party party);

        MethodResult CanAcceptInvite(PartyInvite invite, Citizen citizen);
        void AcceptInvite(PartyInvite invite);
        void DeclineInvite(PartyInvite invite);
        MethodResult CanDeclineInvite(PartyInvite invite, Citizen citizen);

        MethodResult CanSeeInvites(Citizen citizen, Party party);
        MethodResult CanRemoveInvite(PartyInvite invite, Citizen citizen);
        void RemoveInvite(PartyInvite invite);
        PartyRoleEnum GetPartyRole(Entity entity, Party party);

        PartyRoleEnum GetPartyRole(Citizen citizen, Party party);

        MethodResult CanChangeJoinMethod(Citizen citizen, Party party);
        void ChangeJoinMethod(Party party, JoinMethodEnum joinMethod);

        void SendJoinRequest(Citizen citizen, Party party, string message);
        MethodResult CanSendJoinRequest(Citizen citizen, Party party, string message);

        MethodResult CanDeclineJoinRequest(PartyJoinRequest request, Citizen citizen);
        void DeclineJoinRequest(PartyJoinRequest request);
        MethodResult CanCancelJoinRequest(PartyJoinRequest request, Citizen citizen);
        void CancelJoinRequest(PartyJoinRequest request);
        MethodResult CanSeeJoinRequests(Citizen citizen, Party party);
        MethodResult CanAcceptJoinRequest(PartyJoinRequest request, Citizen citizen);
        void AcceptJoinRequest(PartyJoinRequest request);
    }
}
