using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Repository;
using WebServices.Helpers;
using Entities.enums;
using Entities.Extensions;
using Common.Operations;
using Weber.Html;

namespace WebServices
{
    public class PartyService : BaseService, IPartyService
    {
        private readonly IPartyRepository partyRepository;
        private readonly IEntityService entityService;
        private readonly ICitizenRepository citizenRepository;
        private readonly ICongressCandidateVotingRepository congressCandidateVotingRepository;
        private readonly IPartyInviteRepository partyInviteRepository;
        private readonly IPartyJoinRequestRepository partyJoinRequestRepository;
        private readonly IWarningService warningService;
        private readonly IPopupService popupService;


        public PartyService(IPartyRepository partyRepository, IEntityService entityService, ICitizenRepository citizenRepository,
            ICongressCandidateVotingRepository congressCandidateVotingRepository, IPartyInviteRepository partyInviteRepository,
            IPartyJoinRequestRepository partyJoinRequestRepository, IWarningService warningService, IPopupService popupService)
        {
            this.partyRepository = partyRepository;
            this.entityService = entityService;
            this.citizenRepository = citizenRepository;
            this.congressCandidateVotingRepository = congressCandidateVotingRepository;
            this.partyInviteRepository = partyInviteRepository;
            this.partyJoinRequestRepository = partyJoinRequestRepository;
            this.warningService = Attach(warningService);
            this.popupService = popupService;
        }

        public MethodResult CanAcceptInvite(PartyInvite invite, Citizen citizen)
        {
            if (invite == null)
                return new MethodResult("Invite does not exist!");
            if (invite.CitizenID != citizen.ID)
                return new MethodResult("This is not your invite!");

            return MethodResult.Success;
        }

        public MethodResult CanDeclineInvite(PartyInvite invite, Citizen citizen)
        {
            if (invite == null)
                return new MethodResult("Invite does not exist!");
            if (invite.CitizenID != citizen.ID)
                return new MethodResult("This is not your invite!");

            return MethodResult.Success;
        }

        public MethodResult CanChangeJoinMethod(Citizen citizen, Party party)
        {
            if (party == null)
                return new MethodResult("Party does not exist!");
            if (citizen == null)
                return new MethodResult("Citizen does not exist!");

            if (IsPartyMember(citizen, party).IsError)
                return new MethodResult("You are not member of this party!");

            if (GetPartyRole(citizen, party) == PartyRoleEnum.Member)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public void ChangeJoinMethod(Party party, JoinMethodEnum joinMethod)
        {
            party.JoinMethodID = (int)joinMethod;
            DeleteAllPartyInvites(party);

            ConditionalSaveChanges(partyRepository);
        }

        public void DeclineInvite(PartyInvite invite)
        {
            var message = $"{invite.Citizen.Entity.Name} declined party invite.";
            using (NoSaveChanges)
                warningService.AddWarning(invite.PartyID, message);

            partyInviteRepository.Remove(invite);
            ConditionalSaveChanges(partyInviteRepository);
        }

        public void AcceptInvite(PartyInvite invite)
        {
            JoinParty(invite.Citizen, invite.Party);
            partyRepository.SaveChanges();
        }
        public Party CreateParty(string Name, Citizen foundator, Country originCountry)
        {
            var partyEntity = entityService.CreateEntity(Name, EntityTypeEnum.Party);
            var policies = originCountry.CountryPolicy;

            Party party = new Party()
            {
                FoundationDay = GameHelper.CurrentDay,
                Foundator = foundator,
                Entity = partyEntity,
                JoinMethodID = (int)JoinMethodEnum.Request,
                PartyMembers = new List<PartyMember>(),
                Country = originCountry
            };

            var foundatorCongressman = new PartyMember()
            {
                Citizen = foundator,
                PartyRoleID = (int)PartyRoleEnum.President
            };

            party.PartyMembers.Add(foundatorCongressman);

            partyRepository.Add(party);
            partyRepository.SaveChanges();

            CreateNewPresidentVoting(party, GameHelper.CurrentDay + policies.PartyPresidentCadenceLength);

            return party;
        }



        public void InviteCitizen(Citizen citizen, Party party)
        {
            var invitiation = new PartyInvite()
            {
                CitizenID = citizen.ID,
                PartyID = party.ID
            };

            partyInviteRepository.Add(invitiation);
            ConditionalSaveChanges(partyInviteRepository);
        }

        private void SendInviteWarning(Citizen citizen, Party party)
        {
            var partyLink = EntityLinkCreator.Create(party.Entity).ToHtmlString();
            var partyInvites = LinkCreator.Create("party invites", "Invites", "Party");
            var message = $"You were invited to {partyLink}. See {partyInvites} to accept or decline invite.";

            using (NoSaveChanges)
                warningService.AddWarning(citizen.ID, message);
        }

        public MethodResult CanInviteCitizen(Citizen invitingCitizen, Party party, Citizen invitedCitizen)
        {
            if (invitingCitizen == null)
                return new MethodResult("Citizen does not exist!");

            if (party == null)
                return new MethodResult("Party does not exist!");

            if (invitedCitizen == null)
                return new MethodResult("Citizen does not exist!");

            if (invitedCitizen.PartyMember != null)
                return new MethodResult("Citizen is already member of another party!");

            MethodResult result;
            if ((result = IsPartyMember(invitingCitizen, party)).IsError)
                return result;

            if (party.JoinMethodID != (int)JoinMethodEnum.Invite)
                return new MethodResult("Party does not allow to invite people!");

            var invite = partyInviteRepository.FirstOrDefault(i => i.PartyID == party.ID && i.CitizenID == invitedCitizen.ID);

            if (invite != null)
                return new MethodResult("Citizen is already invited!");

            return MethodResult.Success;
        }

        public MethodResult IsPartyMember(Citizen citizen, Party party)
        {
            if(party.PartyMembers.Any(member => member.CitizenID == citizen.ID) == false)
                return new MethodResult("Citizen is not a member of party");

            return MethodResult.Success;
        }

        public PartyMember JoinParty(Citizen who, Party party)
        {
            DeleteAllPartyJoinRequests(who);
            DeleteAllPartyInvites(who);

            var partyMember = new PartyMember()
            {
                Citizen = who,
                Party = party,
                PartyRoleID = (int)PartyRoleEnum.Member
            };

            var citizenLink = EntityLinkCreator.Create(who.Entity).ToHtmlString();
            var message = $"{citizenLink} joined party.";
            using (NoSaveChanges)
                warningService.AddWarning(party.ID, message);

           partyRepository.AddPartyMember(partyMember);
           ConditionalSaveChanges(partyRepository);
            return partyMember;
        }

        public void DeleteAllPartyInvites(Citizen who)
        {
            var invites = partyInviteRepository.Where(invite => invite.CitizenID == who.ID).ToList();
            foreach (var invite in invites)
            {
                InformAboutRemoveInvite(invite);
                partyInviteRepository.Remove(invite);
            }
            ConditionalSaveChanges(partyInviteRepository);
        }

        public void DeleteAllPartyInvites(Party party)
        {
            var invites = partyInviteRepository.Where(invite => invite.PartyID == party.ID).ToList();
            foreach (var invite in invites)
            {
                InformAboutRemoveInvite(invite);
                partyInviteRepository.Remove(invite);
            }
            ConditionalSaveChanges(partyInviteRepository);
        }

        public void ResignFromParty(Citizen who)
        {
            var partyMember = who.PartyMember;
            if (partyMember.PartyRoleID == (int)PartyRoleEnum.President)
                DismissPresidentAndManagers(partyMember.Party);

            RemoveCongressCandidate(partyMember);

            partyRepository.RemovePartyMember(who.PartyMember);
            partyRepository.SaveChanges();

            //Todo : Add some kind of message?
        }

        public void RemoveCongressCandidate(PartyMember member)
        {
            var candidate = congressCandidateVotingRepository.GetCongressCandidateNotApprovedInParty(member.PartyID, member.CitizenID);
            if (candidate != null)
                congressCandidateVotingRepository.RemoveSpecific(candidate);
        }

        public void VoteForPresident(PartyMember member, PartyPresidentCandidate candidate)
        {
            var party = member.Party;

            PartyPresidentVote vote = new PartyPresidentVote()
            {
                CitizenID = member.CitizenID,
                PartyPresidentCandidateID = candidate.ID,
                PartyPresidentVoting = party.GetLastPresidentVoting()
            };

            partyRepository.AddPartyPresidentVote(vote);
            partyRepository.SaveChanges();
        }

        public void DeleteAllPartyJoinRequests(Citizen citizen)
        {
            var requests = citizen.PartyJoinRequests.ToList();

            foreach(var request in requests)
            {
                partyRepository.RemoveJoinRequest(request);
            }

            ConditionalSaveChanges(partyRepository);
        }

        public void ResignFromParty(int citizenID)
        {
            var citizen = citizenRepository.GetById(citizenID);
            ResignFromParty(citizen);
        }

        public void KickFromParty(int kickerCitizenID, int kickedCitizenID)
        {
            var kicker = citizenRepository.GetById(kickerCitizenID);
            var kicked = citizenRepository.GetById(kickedCitizenID);

            partyRepository.RemovePartyMember(kicked.PartyMember);
            partyRepository.SaveChanges();

            //Todo : Add some kind of message?
        }

        public void ProcessDayChange(int newDay)
        {
            var parties = partyRepository.GetAll();
            foreach(var party in parties)
            {
                PartyPresidentVoting partyVoting = party.PartyPresidentVotings.Last();

                if (partyVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted)
                {
                    if (partyVoting.PartyPresidentCandidates.Count == 0)
                    {
                        partyVoting.VotingDay = GameHelper.CurrentDay + 7;
                    }
                    else if (newDay >= partyVoting.VotingDay)
                    {
                        partyVoting.VotingStatusID = (int)VotingStatusEnum.Ongoing;
                    }
                }
                else if (partyVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing)
                {
                    var votes = partyVoting.PartyPresidentVotes
                        .Where(v => v.PartyPresidentCandidate.Citizen.PartyMember != null)
                        .Where(v => v.PartyPresidentCandidate.Citizen.PartyMember.PartyID == party.ID)
                        .GroupBy(v => v.PartyPresidentCandidateID)
                        .Select(v => new { CandidateID = v.Key, VoteCount = v.Count() })
                        .OrderByDescending(v => v.VoteCount)
                        .ToList();

                    if (votes.Count == 0)
                    {
                        partyVoting.VotingStatusID = (int)VotingStatusEnum.Finished;
                        CreateNewPresidentVoting(party, newDay + 7);
                    }



                    else if (votes.Count != 1 && votes[0].VoteCount == votes[1].VoteCount)
                    {

                        partyVoting.VotingStatusID = (int)VotingStatusEnum.Finished;
                        CreateNewPresidentVoting(party, newDay + 7);
                    }
                    else
                    {
                        DismissPresidentAndManagers(party);

                        var policy = party.Country.CountryPolicy;

                        var candidate = partyRepository.GetPartyPresidentCandidate(votes[0].CandidateID);
                        candidate.Citizen.PartyMember.PartyRoleID = (int)PartyRoleEnum.President;

                        CreateNewPresidentVoting(party, newDay + policy.PartyPresidentCadenceLength);
                        partyVoting.VotingStatusID = (int)VotingStatusEnum.Finished;
                    }

                }
            }

            partyRepository.SaveChanges();
        }

        private void ActivatePresidentVoting(Entities.Party party)
        {
            var notStartedVoting = party.PartyPresidentVotings.First(v => v.VotingStatusID == (int)VotingStatusEnum.NotStarted);

            notStartedVoting.VotingStatusID = (int)VotingStatusEnum.Ongoing;

            partyRepository.SaveChanges();
        }

        private PartyPresidentVoting CreateNewPresidentVoting(Entities.Party party, int votingDay)
        {
            PartyPresidentVoting voting = new PartyPresidentVoting()
            {
                Party = party,
                VotingDay = votingDay,
                VotingStatusID = (int)VotingStatusEnum.NotStarted,
            };

            partyRepository.AddPartyPresidentVoting(voting);

            partyRepository.SaveChanges();

            
            return voting;
        }

        public void DismissPresidentAndManagers(Entities.Party party)
        {
            foreach(var management in party.PartyMembers.Where(m => m.PartyRoleID >= (int)PartyRoleEnum.Manager))
            {
                management.PartyRoleID = (int)PartyRoleEnum.Member;
            }
        }

        public void Candidate(Party party, PartyMember member)
        {
            var voting = party.PartyPresidentVotings.Last();
            PartyPresidentCandidate candidate = new PartyPresidentCandidate()
            {
                Citizen = member.Citizen,
                PartyPresidentVoting = voting
            };

            partyRepository.AddPresidentCandidate(candidate);
            partyRepository.SaveChanges();
        }



        public string CanCandidate(Party party, Citizen citizen)
        {
            if (citizen.PartyMember == null || citizen.PartyMember.PartyID != party.ID)
                return "You are not a member of this party";

            var voting = party.PartyPresidentVotings.Last();

            bool isCandidating = voting.PartyPresidentCandidates
                .Any(c => c.CitizenID == citizen.ID);

            if (isCandidating)
                return "You are already candidating!";

            return null;

        }

        public string CanVoteOnCandidate(PartyPresidentCandidate candidate, Citizen voter)
        {
            var voting = candidate.PartyPresidentVoting;

            if (voting.VotingStatusID != (int)VotingStatusEnum.NotStarted)
                return "Wrong candidate!";

            var party = candidate.PartyPresidentVoting.Party;

            if (voter.PartyMember == null || voter.PartyMember.PartyID != party.ID)
                return "You are not a member of this party!";

            if (voting.PartyPresidentVotes.Any(v => v.CitizenID == voter.ID))
                return "You cannot vote again!";

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="citizen">Citizen which we want to check</param>
        /// <param name="party">Party for which citizen will candidate</param>
        /// <returns></returns>
        public bool CanCandidateToCongress(Citizen citizen, Party party)
        {
            if (DoesCitizenBelongToParty(citizen, party) == false)
                return false;

            var country = party.Country;

            var lastCongressVoting = country.GetLastCongressCandidateVoting();

            if (lastCongressVoting.VotingStatusID != (int)VotingStatusEnum.NotStarted)
                return false;

            if (citizen.Entity.GetCurrentRegion().CountryID != country.ID)
                return false;

            return lastCongressVoting.CongressCandidates.Any(c => c.CandidateID == citizen.ID) == false;
        }

        public bool DoesCitizenBelongToParty(Citizen citizen, Party party)
        {
            return citizen.PartyMember != null && citizen.PartyMember.PartyID == party.ID;
        }

        public CongressCandidate CandidateToCongress(Citizen citizen)
        {
            var party = citizen.PartyMember.Party;
            var country = party.Country;
            var region = citizen.Entity.GetCurrentRegion();
            var lastVoting = country.GetLastCongressCandidateVoting();

            CongressCandidate candidate = new CongressCandidate()
            {
                CandidateID = citizen.ID,
                CongressCandidateStatusID = (int)CongressCandidateStatusEnum.WaitingForApproval,
                CongressCandidateVotingID = lastVoting.ID,
                PartyID = party.ID,
                RegionID = region.ID
            };

            congressCandidateVotingRepository.AddCandidate(candidate);
            congressCandidateVotingRepository.SaveChanges();

            return candidate;
        }

        public bool CanAcceptCongressCandidates(Citizen citizen, Party party)
        {
            if(citizen.PartyMember != null)
            {
                var partyMember = citizen.PartyMember;
                if (partyMember.PartyID == party.ID
                    && partyMember.Is(PartyRoleEnum.President))
                    return true;
            }
            return false;
        }

        public MethodResult CanSeeInvites(Citizen citizen, Party party)
        {
            if (party == null)
                return new MethodResult("Party does not exist!");

            if (party.PartyMembers.Any(member => member.CitizenID == citizen.ID) == false)
                return new MethodResult("You are not a member of the party to see this!");

            return MethodResult.Success;
        }

        public PartyRoleEnum GetPartyRole(Entity entity, Party party)
        {
            if (entity.Is(EntityTypeEnum.Citizen) == false)
                return PartyRoleEnum.NotAMember;

            return GetPartyRole(entity.Citizen, party);
        }
        public PartyRoleEnum GetPartyRole(Citizen citizen, Party party)
        {
            return party.PartyMembers
                .Where(member => member.CitizenID == citizen.ID)
                .Select(member => (PartyRoleEnum?)member.PartyRoleID)
                .FirstOrDefault() ?? PartyRoleEnum.NotAMember;
        }

        public MethodResult CanRemoveInvite(PartyInvite invite, Citizen citizen)
        {
            if (invite == null)
                return new MethodResult("Invite does not exist!");

            if(IsPartyMember(citizen, invite.Party).IsError)
                return new MethodResult("You cannot do that!");

            if (GetPartyRole(citizen, invite.Party) == PartyRoleEnum.Member)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public void RemoveInvite(PartyInvite invite)
        {
            InformAboutRemoveInvite(invite);

            partyInviteRepository.Remove(invite);
            partyInviteRepository.SaveChanges();
        }

        private void InformAboutRemoveInvite(PartyInvite invite)
        {
            var partyLink = EntityLinkCreator.Create(invite.Party.Entity).ToHtmlString();
            string message = $"You are no longer invited to {partyLink}.";
            using (NoSaveChanges)
                warningService.AddWarning(invite.CitizenID, message);
        }

        public void AcceptCongressCandidate(CongressCandidate candidate)
        {
            candidate.CongressCandidateStatusID = (int)CongressCandidateStatusEnum.Approved;

            var link = EntityLinkCreator.Create(candidate.Party.Country.Entity).ToHtmlString();
            warningService.AddWarning(candidate.CandidateID,
                message: $"You were accept by your Party President as congress candidate in {link}");

            congressCandidateVotingRepository.SaveChanges();
        }

        public MethodResult CanSendJoinRequest(Citizen citizen, Party party, string message)
        {
            if (citizen == null)
                return new MethodResult("Citizen does not exist!");
            if (party == null)
                return new MethodResult("Party does not exist!");
            if (party.GetJoinMethod() != JoinMethodEnum.Request)
                return new MethodResult("Party does not accept join requests!");
            if (citizen.PartyJoinRequests.Any(request => request.PartyID == party.ID))
                return new MethodResult("You already have join request in this party!");
            if (message.Length > 250)
                return new MethodResult("Message is too long!");

            return MethodResult.Success;

        }

        public void SendJoinRequest(Citizen citizen, Party party, string message)
        {
            var request = new PartyJoinRequest()
            {
                Day = GameHelper.CurrentDay,
                CitizenID = citizen.ID,
                DateTime = DateTime.Now,
                PartyID = party.ID,
                Message = message
            };
            var citizenLink = EntityLinkCreator.Create(citizen.Entity);
            var requestsLink = LinkCreator.Create("join requests", "JoinRequests", "Party", new { partyID = party.ID });

            var warningMessage = $"{citizenLink} send join requests. You can see actual {requestsLink} to accept or decline them.";
            using (NoSaveChanges)
                warningService.AddWarning(party.ID, warningMessage);

            partyJoinRequestRepository.Add(request);
            ConditionalSaveChanges(partyJoinRequestRepository);
        }

        public void AcceptJoinRequest(PartyJoinRequest request)
        {
            var partyLink = EntityLinkCreator.Create(request.Party.Entity);
            var message = $"Your join request to party {partyLink} was accepted";

            using (NoSaveChanges)
            {
                JoinParty(request.Citizen, request.Party);
                warningService.AddWarning(request.CitizenID, message);
            }
           

            partyJoinRequestRepository.SaveChanges();
        }

        public MethodResult CanAcceptJoinRequest(PartyJoinRequest request, Citizen citizen)
        {
            if (request == null)
                return new MethodResult("Request does not exist!");

            if (IsPartyMember(citizen, request.Party).IsError)
                return new MethodResult("Citizen is not a member of the party!");

            if (GetPartyRole(citizen, request.Party) < PartyRoleEnum.Manager)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public MethodResult CanDeclineJoinRequest(PartyJoinRequest request, Citizen citizen)
        {
            if (request == null)
                return new MethodResult("Request does not exist!");

            if (IsPartyMember(citizen, request.Party).IsError)
                return new MethodResult("Citizen is not a member of the party!");

            if (GetPartyRole(citizen, request.Party) == PartyRoleEnum.Member)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public void DeclineJoinRequest(PartyJoinRequest request)
        {
            var partyLink = EntityLinkCreator.Create(request.Party.Entity);
            var message = $"Your request to {partyLink} was declined.";

            using (NoSaveChanges)
                warningService.AddWarning(request.CitizenID, message);

            partyJoinRequestRepository.Remove(request);
            partyJoinRequestRepository.SaveChanges();
        }

        public MethodResult CanCancelJoinRequest(PartyJoinRequest request, Citizen citizen)
        {
            if (request == null)
                return new MethodResult("Request does not exist!");
            if (request.CitizenID != citizen.ID)
                return new MethodResult("You cannot remove someone's request!");

            return MethodResult.Success;
        }

        public void CancelJoinRequest(PartyJoinRequest request)
        {
            var citizenLink = EntityLinkCreator.Create(request.Citizen.Entity);
            var message = $"{citizenLink} canceled join request";

            using (NoSaveChanges)
                warningService.AddWarning(request.PartyID, message);

            partyJoinRequestRepository.Remove(request);
            partyJoinRequestRepository.SaveChanges();
        }

        public MethodResult CanSeeJoinRequests(Citizen citizen, Party party)
        {
            if (party == null)
                return new MethodResult("Party does not exist!");

            PartyRoleEnum role = GetPartyRole(citizen, party);
            if (role == PartyRoleEnum.NotAMember)
                return new MethodResult("You are not a member of the party!");
            if (party.GetJoinMethod() != JoinMethodEnum.Request)
                return new MethodResult("Party does not accept requests!");
            if (role  <= PartyRoleEnum.Manager)
                return new MethodResult("You must be at least manager to view join requests!");            

            return MethodResult.Success;
        }
    }
}
