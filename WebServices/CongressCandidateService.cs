using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using Entities.Repository;
using WebServices.Helpers;
using Entities.Extensions;

namespace WebServices
{
    public class CongressCandidateService : ICongressCandidateService
    {
        private readonly ICongressCandidateVotingRepository congressCandidateVotingRepository;
        private readonly ICountryRepository countryRepository;
        private readonly ICongressmenRepository congressmenRepository;
        private readonly ICitizenService citizenService;

        public CongressCandidateService(
            ICongressCandidateVotingRepository congressCandidateVotingRepository, ICountryRepository countryRepository, ICongressmenRepository congressmenRepository,
            ICitizenService citizenService)
        {
            this.congressCandidateVotingRepository = congressCandidateVotingRepository;
            this.countryRepository = countryRepository;
            this.congressmenRepository = congressmenRepository;
            this.citizenService = citizenService;
        }

        public void ChangeCandidateStatus(CongressCandidate candidate, CongressCandidateStatusEnum status)
        {
            candidate.CongressCandidateStatusID = (int)status;

        }

        public void Resign(CongressCandidate candidate)
        {
            congressCandidateVotingRepository.RemoveCandidate(candidate);
            congressCandidateVotingRepository.SaveChanges();
        }

        public int GetCountryMaxCongressmenAmount(Country country)
        {
            return Math.Max(10, 7 + country.Regions.Count / 2);
        }

        public void VoteOnCongressCandidate(Citizen voter, CongressCandidate candidate)
        {
            CongressCandidateVote vote = new CongressCandidateVote()
            {
                CitizenID = voter.ID,
                CongressCandidateVotingID = candidate.CongressCandidateVotingID,
                SelectedCandidateID = candidate.ID
            };

            congressCandidateVotingRepository.AddVote(vote);
            congressCandidateVotingRepository.SaveChanges();
        }

        public void ProcessDayChange(int newDay)
        {
            var countries = countryRepository.GetAll();
            foreach (var country in countries)
            {
                var policy = country.CountryPolicy;
                var congressCandidateVoting = congressCandidateVotingRepository.GetLastVotingForCountry(country.ID);
                if (congressCandidateVoting.VotingStatusID == (int)VotingStatusEnum.NotStarted)
                {
                    if (congressCandidateVoting.CongressCandidates.Count == 0)
                    {
                        congressCandidateVoting.VotingDay = GameHelper.CurrentDay + 7;
                    }
                    else if (newDay >= congressCandidateVoting.VotingDay)
                    {
                        DismissCongressmen(country);
                        congressCandidateVoting.VotingStatusID = (int)VotingStatusEnum.Ongoing;
                    }
                }
                else if (congressCandidateVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing)
                {
                    var RegionsOfCandidates = congressCandidateVoting
                        .CongressCandidates
                        .Where(c => c.CongressCandidateVoting.CountryID == country.ID)
                        .Where(c => c.CongressCandidateStatusID == (int)CongressCandidateStatusEnum.Approved)
                        .GroupBy(c => c.RegionID)
                        .ToList();

                    foreach(var region in RegionsOfCandidates)
                    {
                        var votes = region
                        .Select(c => new { CandidateID = c.ID, VoteCount = c.CongressCandidateVotes.Count() })
                        .OrderByDescending(v => v.VoteCount)
                        .ThenByDescending(v => v.CandidateID) // candidates which candidated first can win with candidate with same amount of votes
                        .ToList();

                        for (int i = 0; i < Constants.ElectedCongressCandidatesByRegion && i < votes.Count; ++i)
                        {
                            var candidate = congressCandidateVotingRepository.GetCandidate(votes[i].CandidateID);
                            country.Congressmen.Add(createCongressman(candidate.Citizen.ID, country.ID));

                            var gold = GetGoldForCadency(policy.CongressCadenceLength);
                            citizenService.ReceiveCongressMedal(candidate.Citizen, gold);
                        }
                    }

                    congressCandidateVoting.VotingStatusID = (int)VotingStatusEnum.Finished;

                    if (RegionsOfCandidates.Count == 0)
                    {
                        CreateNewCongressCandidateVoting(country, newDay + 7);
                    } else
                    {
                        CreateNewCongressCandidateVoting(country, newDay + policy.CongressCadenceLength);
                    }
                }
            }

            congressCandidateVotingRepository.SaveChanges();
        }

        private void DismissCongressmen(Country country)
        {
            congressmenRepository.RemoveRange(c => c.CountryID == country.ID);
            //TODO : Sent notifications about cadence end?
        }

        private Congressman createCongressman(int citizenID, int countryID)
        {
            return new Congressman()
            {
                CitizenID = citizenID,
                CountryID = countryID,
                LastVotingDay = 0
            };
        }

        public CongressCandidateVoting CreateNewCongressCandidateVoting(Country country, int votingDay)
        {
            CongressCandidateVoting voting = new CongressCandidateVoting()
            {
                CountryID = country.ID,
                VotingDay = votingDay,
                VotingStatusID = (int)VotingStatusEnum.NotStarted
            };

            congressCandidateVotingRepository.Add(voting);

            congressCandidateVotingRepository.SaveChanges();


            return voting;
        }

        /// <summary>
        /// Country, Party information will be taken from citizen
        /// </summary>
        /// <param name="citizen"></param>
        /// <returns></returns>
        public CongressCandidate CreateNewCongressCandidate(Citizen citizen)
        {
            var party = citizen.PartyMember.Party;
            var country = party.Country;

            var lastVoting = country.GetLastCongressCandidateVoting();

            var candidate = new CongressCandidate()
            {
                PartyID = party.ID,
                RegionID = citizen.RegionID,
                CandidateID = citizen.ID,
                CongressCandidateStatusID = (int)CongressCandidateStatusEnum.WaitingForApproval,
                CongressCandidateVotingID = lastVoting.ID
            };

            congressCandidateVotingRepository.AddCandidate(candidate);
            congressCandidateVotingRepository.SaveChanges();

            return candidate;
        }

        public bool CanVote(Citizen citizen, CongressCandidateVoting voting)
        {
            return voting.CongressCandidateVotes
                .Any(v => v.CitizenID == citizen.ID) == false;
        }

        public double GetGoldForCadency(int cadencyLength)
        {
            return Math.Round(((double)cadencyLength / (double)Constants.CongressCadenceDefaultLength) * Constants.CongressCadenceMedalGold, 2);
        }
    }
}
