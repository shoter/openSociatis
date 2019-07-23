using Common.Exceptions;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Congress;
using Sociatis.Models.Congress.Votings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebUtils;
using WebUtils.Attributes;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    public class CongressController : ControllerBase
    {
        ICongressVotingRepository congressVotingRepository;
        ICongressVotingService congressVotingService;
        ICongressCandidateVotingRepository congressCandidateVotingRepository;
        ICongressCandidateService congressCandidateService;
        ICountryRepository countryRepository;
        IRegionRepository regionRepository;
        IPartyRepository partyRepository;
        ICompanyRepository companyRepository;
        private readonly IEntityRepository entityRepository;


        public CongressController(ICongressVotingRepository congressVotingRepository, ICongressVotingService congressVotingService, ICountryRepository countryRepository
            , IRegionRepository regionRepository, ICongressCandidateVotingRepository congressCandidateVotingRepository,
            ICongressCandidateService congressCandidateService, IPartyRepository partyRepository, IEntityRepository entityRepository
            , IPopupService popupService, ICompanyRepository companyRepository) : base(popupService)
        {
            this.congressVotingRepository = congressVotingRepository;
            this.congressVotingService = congressVotingService;
            this.countryRepository = countryRepository;
            this.regionRepository = regionRepository;
            this.congressCandidateVotingRepository = congressCandidateVotingRepository;
            this.congressCandidateService = congressCandidateService;
            this.partyRepository = partyRepository;
            this.entityRepository = entityRepository;
            this.companyRepository = companyRepository;
        }


        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Congress/StartVoting")]
        public ActionResult StartVoting(int countryID)
        {
            var entity = SessionHelper.CurrentEntity;

            if (entity.Is(EntityTypeEnum.Citizen) == false)
                return RedirectBackWithError("You are not a citizen.");

            var citizen = entity.Citizen;
            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectBackWithError("Country does not exist!");

            if (congressVotingService.IsCongressman(citizen, country) == false)
                return RedirectBackWithError("You are not a party member of this country!");

            if (citizen.Congressmen.First(c => c.CountryID == countryID).LastVotingDay >= GameHelper.CurrentDay)
            {
                AddError("You started voting today!");
                return RedirectToAction("Votings", new { countryID = countryID });
            }

            if (country.Congressmen.Count < 3)
            {
                AddWarning("You cannot vote when there is less than 3 congressmen. [Feature will be enabled in release version. You can vote without problems]");
            }


            var votingTypes = congressVotingRepository.GetVotingTypes();
            var commentRestrictions = congressVotingRepository.GetCommentRestrictions();


            var vm = new CongressStartVotingViewModel(votingTypes, commentRestrictions, country);
            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Congress/StartVoting")]
        public ActionResult StartVoting(int countryID, FormCollection values)
        {
            try
            {
                var entity = SessionHelper.CurrentEntity;

                if (entity.Is(EntityTypeEnum.Citizen) == false)
                    return RedirectBackWithError("You are not a citizen.");

                var citizen = entity.Citizen;
                var country = countryRepository.GetById(countryID);

                if (country == null)
                    return RedirectBackWithError("Country does not exist!");

                if (congressVotingService.IsCongressman(citizen, country) == false)
                    return RedirectBackWithError("You are not a party member of this country!");

                if (citizen.Congressmen.First(c => c.CountryID == countryID).LastVotingDay >= GameHelper.CurrentDay)
                {
                    AddError("You started voting today!");
                    return RedirectToAction("Votings", new { countryID = countryID });
                }

                if (country.Congressmen.Count < 3)
                {
                    AddWarning("You cannot vote when there is less than 3 congressmen. [Feature will be enabled in release version. You can vote without problems]");
                }

                var votingTypes = congressVotingRepository.GetVotingTypes();
                var commentRestrictions = congressVotingRepository.GetCommentRestrictions();


                var vm = new CongressStartVotingViewModel(votingTypes, commentRestrictions, country);

                var votingType = (VotingTypeEnum)int.Parse(values["VotingType"]);

                var vote = CongressVotingViewModelChooser.GetViewModel(votingType, values);



                if (TryValidateModel(vote, "EmbeddedVote"))
                {

                    CommentRestrictionEnum commentRestriction = (CommentRestrictionEnum)int.Parse(values["CommentRestriction"]);
                    var parameters = vote.CreateVotingParameters();

                    parameters.CommentRestriction = commentRestriction;
                    parameters.Country = country;
                    parameters.Creator = citizen;
                    parameters.CreatorMessage = vote.Message;
                    parameters.VotingLength = country.CountryPolicy.CongressVotingLength;

                    var voting = congressVotingService.StartVote(parameters);

                    return RedirectToAction("ViewVoting", new { votingID = voting.ID });


                }
                vm.EmbedPostVote(vote);
                return View(vm);
            }
#if !DEBUG
            catch(Exception e)
            {
                if(e is UserReadableException)
                    AddError(e.Message);
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectBack();
            }
#else

            catch (UserReadableException e)
            {
                AddError(e.Message);
                return RedirectBack();
            }
#endif

        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Congress/Members")]
        public ActionResult Members(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectToHomeWithError("Country does not exist!");

            var congressmen = country.Congressmen
                .ToList();

            var vm = new CongressMemberSectionViewModel(congressmen, country, partyRepository);

            return View(vm);
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Congress/Candidates")]
        public ActionResult CongressCandidates(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            if (country == null)
                return RedirectToHomeWithError("Country does not exist!");

            var vm = new CongressCandidateSectionViewModel(country);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CongressCandidatesForRegion(int regionID)
        {
            var region = regionRepository.GetById(regionID);
            var country = region.Country;

            if (country.Regions.Any(r => r.ID == regionID) == false)
                return ServerError("Region does not exist!");

            var lastVoting = country.GetLastCongressCandidateVoting();

            var candidates = lastVoting
                .CongressCandidates
                .Where(c => c.Is(CongressCandidateStatusEnum.Approved))
                .Where(cc => cc.RegionID == regionID)
                .ToList();



            var vm = new CongressCandidatesListViewModel(candidates,
                canVote: SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen)
                && lastVoting.HasVoted(SessionHelper.LoggedCitizen) == false &&
                lastVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing);

            return PartialView(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult VoteCongressCandidate(int candidateID)
        {
            var candidate = congressCandidateVotingRepository.GetCandidate(candidateID);
            var voting = candidate.CongressCandidateVoting;

            if (voting.Is(VotingStatusEnum.NotStarted))
                return RedirectBackWithError("Voting is not started yet!");
            else if (voting.Is(VotingStatusEnum.Finished))
                return RedirectBackWithError("Voting was finished");

            var entity = SessionHelper.CurrentEntity;

            if (entity.Is(EntityTypeEnum.Citizen) == false)
                return RedirectBackWithError("You are not a citizen");

            var citizen = SessionHelper.LoggedCitizen;

            if (voting.HasVoted(citizen))
                return RedirectBackWithError("You alerady voted!");

            congressCandidateService.VoteOnCongressCandidate(citizen, candidate);
            AddInfo(string.Format("You voted on {0}.", candidate.Citizen.Entity.Name));

            return RedirectBack();
        }



        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CongressCandidatesForAllRegions(int countryID)
        {
            var country = countryRepository.GetById(countryID);

            var lastVoting = country.GetLastCongressCandidateVoting();

            var candidates = lastVoting
                .CongressCandidates
                .Where(c => c.Is(CongressCandidateStatusEnum.Approved))
                .ToList();

            var vm = new CongressCandidatesListViewModel(candidates,
                canVote: SessionHelper.CurrentEntity.Is(EntityTypeEnum.Citizen)
                && lastVoting.HasVoted(SessionHelper.LoggedCitizen) == false &&
                lastVoting.VotingStatusID == (int)VotingStatusEnum.Ongoing);

            return PartialView(vm);
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Congress")]
        public ActionResult View(int countryID)
        {
            var country = countryRepository.GetById(countryID);
            var vm = new ViewCongressViewModel(country);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Congress/Voting/{votingID:int}")]
        public ActionResult ViewVoting(int votingID)
        {
            var voting = congressVotingRepository.GetById(votingID);

            if (voting == null)
                return RedirectBackWithError("Voting does not exist!");

            var vm = CongressViewVotingViewModelChooser.Instantiate(voting, congressVotingService);

            return View(vm);

        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Candidate(int countryID)
        {
            var citizen = SessionHelper.CurrentEntity.Citizen;
            if (citizen == null)
                return RedirectToHomeWithError("You are not a citizen!");
            if (citizen.PartyMember == null)
            {
                AddError("You are not a party member to be able to become congress candidate");
                return RedirectToAction("View", new { countryID = countryID });
            }
            else if (citizen.PartyMember.Party.CountryID != countryID)
            {
                var country = entityRepository.GetById(countryID);
                AddError("You are a party member in diffrent country. You cannot candidate in {0}", country.Name);
                return RedirectToAction("View", new { countryID = countryID });
            }

            return RedirectToAction("CongressCandidates", "Party", new { partyID = citizen.PartyMember.PartyID });

        }




        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public PartialViewResult DisplayVotingInfo(VotingTypeEnum votingType, int votingID)
        {
            var voting = congressVotingRepository.GetById(votingID);
            CongressVotingViewModel vm = CongressVotingViewModelChooser.GetViewModel(votingType, voting);

            return PartialView(vm);
        }

        [HttpGet]
        [AjaxOnly]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public PartialViewResult EditOptionForm(VotingTypeEnum votingType, int countryID)
        {
            CongressVotingViewModel vm = CongressVotingViewModelChooser.GetViewModel(votingType, countryID);
            return PartialView(vm);
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Country/{countryID:int}/Congress/Votings")]
        public ActionResult Votings(int countryID)
        {


            var country = countryRepository.GetById(countryID);
            if (country == null)
                return RedirectBackWithError("Country not found!");


            var vm = new CongressVotingsViewModel(country);

            return View(vm);
        }

        public PartialViewResult VotingsPartial(int countryID, PagingParam pagingParam)
        {
            pagingParam = pagingParam ?? new PagingParam();

            var votingsQuery = congressVotingRepository.Where(cv => cv.CountryID == countryID);

            var vm = new CongressVotingsPartialViewModel(countryID, votingsQuery, pagingParam);

            return PartialView(vm);
        }



        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult AddCongressVotingComment(int congressVotingID, string message)
        {
            try
            {
                var voting = congressVotingRepository.GetById(congressVotingID);
                var entity = SessionHelper.CurrentEntity;

                if (entity.Citizen == null)
                    return JsonError("Wrong entity");

                var citizen = entity.Citizen;

                if (voting.CommentRestrictionID != (int)CommentRestrictionEnum.CitizenCanComment && voting.Country.Congressmen.Any(c => c.CitizenID == citizen.ID) == false)
                    return JsonError("You cannot comment this");


                congressVotingService.AddComment(voting, citizen, message);

                return JsonSuccess("Added comment");
            }
            catch (Exception e)
            {
#if DEBUG
                return JsonError(e.Message);
#else
                return JsonError("Error");
#endif
            }
        }

        public PartialViewResult CongressVotingComments(int congressVotingID, PagingParam pagingParam)
        {
            var comments = congressVotingRepository.GetVotingComments(congressVotingID);
            var voting = congressVotingRepository.GetById(congressVotingID);

            var citizen = SessionHelper.LoggedCitizen;
            var country = voting.Country;

            if (voting.CommentRestrictionID == (int)CommentRestrictionEnum.OnlyCongressmen && country.Congressmen.Any(c => c.CitizenID == citizen.ID) == false)
                throw new Exception();

            var vm = new CongressVotingCommentListViewModel(pagingParam, comments, voting);

            return PartialView(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult VoteFor(int congressVotingID)
        {
            return Vote(congressVotingID, VoteTypeEnum.Yes);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult VoteAgainst(int congressVotingID)
        {
            return Vote(congressVotingID, VoteTypeEnum.No);
        }

        private ActionResult Vote(int congressVotingID, VoteTypeEnum voteType)
        {
            var voting = congressVotingRepository.GetById(congressVotingID);
            var entity = SessionHelper.CurrentEntity;

            if (entity.Citizen == null)
            {
                AddError("Wrong entity");
                return RedirectToAction("ViewVoting", new { votingID = congressVotingID });
            }

            var citizen = entity.Citizen;

            if (voting.CommentRestrictionID != (int)CommentRestrictionEnum.CitizenCanComment && voting.Country.Congressmen.Any(c => c.CitizenID == citizen.ID) == false)
            {
                AddError("You cannot vote on this!");
                return RedirectToAction("ViewVoting", new { votingID = congressVotingID });
            }

            if (congressVotingService.CanVote(citizen, voting) == false)
            {
                AddError("You cannot vote!");
                return RedirectToAction("ViewVoting", new { votingID = congressVotingID });
            }

            if (voting.GetTimeLeft(GameHelper.CurrentDay).TotalSeconds <= 0)
            {
                AddError("You cannot vote!");
                return RedirectToAction("ViewVoting", new { votingID = congressVotingID });
            }

            congressVotingService.AddVote(voting, citizen, voteType);

            AddInfo("You had voted");
            return RedirectToAction("ViewVoting", new { votingID = congressVotingID });
        }

        public JsonResult GetTransferMoneyToCompanies(Select2Request request, int countryID)
        {
            string query = (request.Query ?? "").Trim().ToLower();

            var companies = companyRepository.Where(company => company.Entity.Name.ToLower().Contains(query))
                .OrderBy(company => company.Entity.Name)
                .Select(company => new Select2Item()
                {
                    id = company.ID,
                    text = company.Entity.Name
                });

            return Select2Response(companies, request);
        }

    }
}