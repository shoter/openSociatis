using Common.Exceptions;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models;
using Sociatis.Models.Party;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.enums;
using WebServices.Helpers;
using WebServices.structs;
using Sociatis.Models.Congress;
using Entities.Extensions;
using Entities;
using Sociatis.Code.Filters;
using Common.Operations;
using WebUtils;
using WebUtils.Extensions;
using System.Web.Routing;
using WebServices.Models;
using System.Data.Entity;
using WebUtils.Attributes;
using Sociatis.Code.Json;
using Weber.Html;
using Sociatis.Validators.Parties;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class PartyController : ControllerBase
    {
        private readonly IPartyRepository partyRepository;
        private readonly IPartyService partyService;
        private readonly ITransactionsService transactionService;
        private readonly IUploadService uploadService;
        private readonly ICongressCandidateVotingRepository congressCandidateVotingRepository;
        private readonly IWarningService warningService;
        private readonly ICountryRepository countryRepository;
        private readonly IEntityService entityService;
        private readonly ICongressCandidateService congressCandidateService;
        private readonly ICitizenRepository citizenRepository;
        private readonly IPartyInviteRepository partyInviteRepository;
        private readonly IPartyJoinRequestRepository partyJoinRequestRepository;
       
        public PartyController(IPartyRepository partyRepository, IPartyService partyService, ITransactionsService transactionService,
            IUploadService uploadService, ICongressCandidateVotingRepository congressCandidateVotingRepository, IWarningService warningService,
            ICountryRepository countryRepository, IEntityService entityService, ICongressCandidateService congressCandidateService
            , IPopupService popupService, ICitizenRepository citizenRepository, IPartyInviteRepository partyInviteRepository,
            IPartyJoinRequestRepository partyJoinRequestRepository) : base(popupService)
        {
            this.partyRepository = partyRepository;
            this.partyService = partyService;
            this.transactionService = transactionService;
            this.uploadService = uploadService;
            this.congressCandidateVotingRepository = congressCandidateVotingRepository;
            this.warningService = warningService;
            this.countryRepository = countryRepository;
            this.entityService = entityService;
            this.congressCandidateService = congressCandidateService;
            this.citizenRepository = citizenRepository;
            this.partyInviteRepository = partyInviteRepository;
            this.partyJoinRequestRepository = partyJoinRequestRepository;
        }

        [AjaxOnly]
        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Party)]
        public JsonResult AcceptInvite(int inviteID)
        {
            var invite = partyInviteRepository.GetById(inviteID);
            var citizen = SessionHelper.LoggedCitizen;
            MethodResult result;
            if ((result = partyService.CanAcceptInvite(invite, citizen)).IsError)
                return JsonError(result);
            partyService.AcceptInvite(invite);

            return JsonRedirect(nameof(View), "Party", new { partyID = invite.PartyID });
        }

        [AjaxOnly]
        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Party)]
        public JsonResult RemoveInvite(int citizenID, int partyID)
        {
            var invite = partyInviteRepository.FirstOrDefault(i => i.CitizenID == citizenID && i.PartyID == partyID);
            MethodResult result;
            if ((result = partyService.CanRemoveInvite(invite, SessionHelper.LoggedCitizen)).IsError)
                return JsonError(result);
            var citizenName = invite.Citizen.Entity.Name;
            partyService.RemoveInvite(invite);
            return JsonSuccess($"You successfully removed {citizenName} invite.");
        }

        [NoAjax]
        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Party)]
        [Route("Party/{inviteID:int}/AcceptInvite")]
        public ActionResult AcceptInviteNoAjax(int inviteID)
        {
            var invite = partyInviteRepository.GetById(inviteID);
            var citizen = SessionHelper.LoggedCitizen;
            MethodResult result;
            if ((result = partyService.CanAcceptInvite(invite, citizen)).IsError)
                return RedirectToHomeWithError(result);
            partyService.AcceptInvite(invite);

            return RedirectToAction(nameof(View), new { partyID = invite.PartyID });
        }

        [AjaxOnly]
        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Party)]
        public JsonResult DeclineInvite(int inviteID)
        {
            var invite = partyInviteRepository.GetById(inviteID);
            var citizen = SessionHelper.LoggedCitizen;
            MethodResult result;
            if ((result = partyService.CanDeclineInvite(invite, citizen)).IsError)
                return JsonError(result);
            partyService.DeclineInvite(invite);

            return JsonSuccess("You declined party invite.");
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult UploadAvatar(HttpPostedFileBase avatarFile, int partyID)
        {
            var party = partyRepository.GetById(partyID);
            var citizen = SessionHelper.LoggedCitizen;
            var partyMember = citizen.PartyMember;

            var partyRoles = partyRepository.GetPartyRoles();
            var joinMethods = partyRepository.GetJoinMethods();

            if (partyMember == null || partyMember.PartyID != partyID)
            {
                return RedirectIfNotMember(partyID);
            }

            var partyRole = (PartyRoleEnum)partyMember.PartyRoleID;
            if(partyRole < PartyRoleEnum.Manager)
            {
                AddError("You cannot change the avatar!");
                return RedirectToAction("View", new { partyID = partyID });
            }

            try
            {
                MethodResult<string> fileName = uploadService.UploadImage(avatarFile, UploadLocationEnum.Avatars);
                if(fileName.IsError)
                {
                    AddError(fileName as MethodResult);

                }
                if (string.IsNullOrWhiteSpace(fileName) == false)
                {
                    if(VirtualFileHelper.Exists(party.Entity.ImgUrl))
                    {
                        VirtualFileHelper.Delete(party.Entity.ImgUrl);
                    }
                    party.Entity.ImgUrl = fileName;
                    partyRepository.SaveChanges();
                }
                else
                {
                    AddError("Something went wrong with adding the avatar");
                }

            }
            catch(UserReadableException e)
            {
                AddError(e.Message);
            }
            catch (Exception)
            {
                AddError("Something went wrong with adding the avatar");
            }


            return RedirectToAction("View", new { partyID = partyID });
        }

        
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Party)]
        [Route("Party/Invites")]
        public ActionResult Invites()
        {
            var citizen = SessionHelper.LoggedCitizen;

            var invites = partyInviteRepository.Where(invite => invite.CitizenID == citizen.ID)
                .Include(invite => invite.Citizen.Entity)
                .Include(invite => invite.Party.Entity);

            var vm = new PartyInviteListViewModel(invites);

            return View(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Invite(int partyID, int citizenID)
        {
            throw new NotImplementedException("now we have AJAX action for that");
/*
            var currentCitizen = SessionHelper.CurrentEntity;
            if (currentCitizen.GetEntityType() != EntityTypeEnum.Citizen)
                return RedirectToHomeWithError("You are not a citizen!");

            var party = partyRepository.GetById(partyID);
            var citizen = citizenRepository.GetById(citizenID);
            MethodResult result;
            if ((result = partyService.CanInviteCitizen(SessionHelper.LoggedCitizen, party, citizen)).IsError)
                return RedirectBackWithError(result);

            partyService.InviteCitizen(citizen, party);
            AddSuccess($"You invited {citizen.Entity.Name} to {party.Entity.Name}");
            return RedirectBack();
*/
        }

        [AjaxOnly]
        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public JsonResult InviteAjax(int partyID, int citizenID)
        {
            var currentCitizen = SessionHelper.CurrentEntity;
            if (currentCitizen.GetEntityType() != EntityTypeEnum.Citizen)
                return JsonError("You are not a citizen!");

            var party = partyRepository.GetById(partyID);
            var citizen = citizenRepository.GetById(citizenID);
            MethodResult result;
            if ((result = partyService.CanInviteCitizen(SessionHelper.LoggedCitizen, party, citizen)).IsError)
                return JsonError(result);

            partyService.InviteCitizen(citizen, party);
            return JsonSuccess($"You invited {citizen.Entity.Name} to {party.Entity.Name}");
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Party/{partyID:int}/CongressCandidates")]
        public ActionResult CongressCandidates(int partyID)
        {
            var party = partyRepository.GetById(partyID);

            if (party == null)
                return PartyDoesNotExistRedirect();

            var vm = new PartyCongressCandidateSectionViewModel(party, partyService);

            return View(vm);
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Parties/")]
        public ActionResult PartiesGeneral()
        {
            var country = SessionHelper.CurrentEntity.GetCurrentCountry();

            return RedirectToAction("Parties", new RouteValueDictionary { { "countryID", country.ID }, { "pagingParam.PageNumber", 1 } });
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult ResignAsCandidate()
        {
            var entityID = SessionHelper.CurrentEntity.EntityID;
            var candidate = congressCandidateVotingRepository.GetCandidateInNotStartedVoting(entityID);

            if(candidate != null)
            {
                congressCandidateService.Resign(candidate);
                AddSuccess("You have successfully resigned.");
            }
            else
            {
                AddError("You are not a congress candidate!");
            }

            return RedirectBack();

        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult PartiesPost(int countryID, PagingParam pagingParam)
        {
            return RedirectToAction("MarketOffers", new RouteValueDictionary { { "countryID", countryID }, { "pagingParam.PageNumber", pagingParam.PageNumber } });
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Parties/{countryID:int}/{pagingParam.PageNumber:int=1}")]
        public ActionResult Parties(int countryID, PagingParam pagingParam)
        {
            var country = countryRepository.GetById(countryID);
            pagingParam = pagingParam ?? new PagingParam();
            var parties = partyRepository
                .GetPartiesInCountry(countryID)
                .OrderByDescending(p => p.PartyMembers.Count)
                .ThenBy(p => p.Entity.Name)
                .Apply(pagingParam)
                .ToList();

            var vm = new PartiesListViewModel(country, parties, pagingParam);

            return View(vm);
        }

        

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CongressCandidatesForRegion(int partyID, int regionID)
        {
            var party = partyRepository.GetById(partyID);
            

            if(party == null)
                return ServerError("Party does not exist!");

            var country = party.Country;

            if (country.Regions.Any(r => r.ID == regionID) == false)
                return ServerError("Region does not exist!");

            var lastVoting = country.GetLastCongressCandidateVoting();

            var candidates = lastVoting
                .CongressCandidates
                .Where(c => c.PartyID == partyID)
                .Where(cc => cc.RegionID == regionID)
                .ToList();

            var vm = new PartyCongressCandidatesListViewModel(candidates, partyService, party);

            return PartialView(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult AcceptCongressCandidate(int candidateID)
        {
            var entity = SessionHelper.CurrentEntity;

            if (!entity.Is(EntityTypeEnum.Citizen))
                return RedirectToHomeWithError();

            var citizen = entity.Citizen;

            if(citizen.PartyMember == null)
                return RedirectToHomeWithError();

            var candidate = congressCandidateVotingRepository.GetCandidate(candidateID);
            var party = candidate.Party;

            if (partyService.CanAcceptCongressCandidates(citizen, party))
            {
                
                partyService.AcceptCongressCandidate(candidate);
                AddInfo(string.Format("You accepted {0} as congress candidate", candidate.Citizen.Entity.Name));

                return RedirectToAction("CongressCandidates", new { partyID = party.ID });
            }

            return RedirectToHomeWithError();
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CongressCandidatesForAllRegions(int partyID)
        {
            var party = partyRepository.GetById(partyID);

            if (party == null)
                return ServerError("Party does not exist!");

            var country = party.Country;

            var lastVoting = country.GetLastCongressCandidateVoting();

            var candidates = lastVoting
                .CongressCandidates
                .Where(c => c.PartyID == partyID)
                .ToList();

            var vm = new PartyCongressCandidatesListViewModel(candidates, partyService, party);

            return PartialView(vm);
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Party/{partyID:int}/Candidate")]
        public ActionResult Candidate(int partyID)
        {
            var party = partyRepository.GetById(partyID);
            var entity = SessionHelper.CurrentEntity;

            if(entity.EntityTypeID != (int)EntityTypeEnum.Citizen)
            {
                AddError("Only citizens can candidate!");
                return RedirectToAction("View", new { partyID = partyID });
            }

            var errorMessage = partyService.CanCandidate(party, entity.Citizen);

            if(string.IsNullOrEmpty(errorMessage))
            {
                partyService.Candidate(party, entity.Citizen.PartyMember);

                AddInfo("You are a candidate for upcoming party elections");
                return RedirectToAction("View", new { partyID = partyID });
            }

            AddError(errorMessage);
            return RedirectToAction("View", new { partyID = partyID });
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Party/{partyID:int}/PartyPresidentCandidates")]
        public ActionResult Candidates(int partyID)
        {
            if (checkIfBelongsToParty(partyID, SessionHelper.LoggedCitizen))
            {
                return DoNotBelongToPartyRedirect(partyID);
            }

            var party = partyRepository.GetById(partyID);
            var voting = party.PartyPresidentVotings.Last();

            var vm = new PartyCandidateListViewModel(voting);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen, EntityTypeEnum.Party)]
        [Route("Party/{partyID:int}/JoinRequests")]
        public ActionResult JoinRequests(int partyID)
        {
            var party = partyRepository.GetById(partyID);
            var citizen = SessionHelper.LoggedCitizen;

            MethodResult result;
            if ((result = partyService.CanSeeJoinRequests(citizen, party)).IsError)
                return RedirectBackWithError(result);

            var requests = partyJoinRequestRepository.Where(request => request.PartyID == partyID)
                .Include(request => request.Citizen.Entity)
                .ToList();

            var vm = new PartyJoinRequestsListViewModel(party, requests);

            return View(vm);
        }

        private ActionResult DoNotBelongToPartyRedirect(int partyID)
        {
            AddError("You do not belong to this party!");
            return RedirectToAction("View", new { partyID = partyID });
        }

        private static bool checkIfBelongsToParty(int partyID, Entities.Citizen citizen)
        {
            return citizen.PartyMember == null || citizen.PartyMember.PartyID != partyID;
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult ChangeRoles(List<ManagePartyMemberViewModel> members, int partyID)
        {
            var party = partyRepository.GetById(partyID);

            var citizen = SessionHelper.LoggedCitizen;
            var partyMember = citizen.PartyMember;

            if (partyMember == null || partyMember.PartyID != partyID)
            {
                return RedirectIfNotMember(partyID);
            }

            var partyRole = (PartyRoleEnum)partyMember.PartyRoleID;
            var partyMembers = partyRepository.GetMembers(partyID);

            if (partyRole < PartyRoleEnum.Manager)
            {
                AddError("You cannot manage this party!");
                return RedirectToAction("View", new { partyID = partyID });
            }
            var partyLink = EntityLinkCreator.Create(party.Entity);
            foreach (var member in members)
            {
                var domainModel = partyMembers.FirstOrDefault(m => m.CitizenID == member.CitizenID);
                
                if (partyRole == PartyRoleEnum.President)
                {
                    if (domainModel == null)
                    {
                        AddError(string.Format("Party member with {0} ID does not exists!", member.CitizenID));
                        continue;
                    }

                    if (domainModel.PartyRoleID != (int)member.PartyRole)
                    {
                        warningService.AddWarning(domainModel.CitizenID,
                            string.Format($"Your role in party changed from {0} to {1} in party {partyLink}", member.PartyRole.ToHumanReadable(), party.Entity.Name));

                        domainModel.PartyRoleID = (int)member.PartyRole;
                    }
                }
                if(partyRole >= PartyRoleEnum.Manager)
                {
                    if (domainModel == null)
                    {
                        AddError(string.Format("Party member with {0} ID does not exists!", member.CitizenID));
                        continue;
                    }
                    if (partyRole <= member.PartyRole)
                    {
                        AddError("You cannot kick this user!");
                        continue;
                    }
                    if(member.Kick)
                    {
                        partyService.ResignFromParty(member.CitizenID);
                    }
                }
            }

            AddInfo("Party members updated");
            partyRepository.SaveChanges();
            return RedirectToAction("View", new { partyID = partyID });
            
        }


        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult ChangeJoinMethod(JoinMethodEnum joinMethod, int partyID)
        {
            var party = partyRepository.GetById(partyID);
            var citizen = SessionHelper.LoggedCitizen;

            MethodResult result;
            if ((result = partyService.CanChangeJoinMethod(citizen, party)).IsError)
                return RedirectBackWithError(result);

            partyService.ChangeJoinMethod(party, joinMethod);

            AddInfo("Join method was changed!");
            return RedirectToAction("View", new { partyID = partyID });
        }

        [HttpPost]
        [Route("Party/VotePresident")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult VotePresident(int candidateID)
        {
            var candidate = partyRepository.GetPartyPresidentCandidate(candidateID);

            if(candidate == null)
            {
                AddError("Candidate does not exist");
                return RedirectToAction("Index", "Home");
            }

            var voting = candidate.PartyPresidentVoting;
            var party = partyRepository.GetById(voting.PartyID);
            var entity = SessionHelper.CurrentEntity;
            var citizen = SessionHelper.LoggedCitizen;

            var partyMember = citizen.PartyMember;

            if (entity.EntityID != citizen.ID || partyMember == null || partyMember.PartyID != party.ID)
            {
                return RedirectIfNotMember(party.ID);
            }

            if(voting.PartyPresidentVotes.Any(v => v.CitizenID == citizen.ID))
            {
                AddError("You were already voting!");
                return RedirectToAction("View", new { partyID = party.ID });
            }

            if(voting.PartyID != partyMember.PartyID)
            {
                AddError("Candidate is not a member of your party");
                return RedirectToAction("Index", "Home");
            }

            partyService.VoteForPresident(partyMember, candidate);

            AddInfo(string.Format("You have successfully voted for {0}", candidate.Citizen.Entity.Name));
            return RedirectToAction("Candidates", new { partyID = candidate.PartyPresidentVoting.PartyID });
        }

        [Route("Party/{partyID:int}")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ViewResult View(int partyID)
        {
            var party = partyRepository.GetById(partyID);



            var vm = new ViewPartyViewModel(party);

            return View(vm);
        }

        [HttpPost]
        [Route("Party/Join")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Join(int partyID)
        {
            var party = partyRepository.GetById(partyID);
            var citizen = SessionHelper.LoggedCitizen;
            var partyMember = citizen.PartyMember;

            if(partyMember != null)
                return YouAreAlreadyMemberOfThePartyRedirection();

            switch((JoinMethodEnum)party.JoinMethodID)
            {
                case JoinMethodEnum.Invite:
                    {
                        AddMessage("You cannot join this party", PopupMessageType.Error);
                        return RedirectToAction("View", new { partyID = partyID });
                    }
                case JoinMethodEnum.Open:
                    {
                        partyService.JoinParty(citizen, party);
                        AddSuccess($"Joined party {party.Entity.Name}.");
                        return RedirectToAction("View", new { partyID = partyID });
                    }
                case JoinMethodEnum.Request:
                    {
                        return RedirectToAction("View", new { partyID = partyID });
                    }
                default:
                    return RedirectToAction("View", new { partyID = partyID });
            }
        }

        [HttpPost]
        [Route("Party/Leave")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Leave(int partyID)
        {
            var party = partyRepository.GetById(partyID);
            var citizen = SessionHelper.LoggedCitizen;
            var partyMember = citizen.PartyMember;

            if (partyMember == null || partyMember.PartyID != partyID)
            {
                return RedirectIfNotMember(partyID);
            }

            partyService.ResignFromParty(citizen);

            AddMessage("You left the party", PopupMessageType.Info);
            return RedirectToHome();
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult CandidateToCongress()
        {
            var citizen = SessionHelper.CurrentEntity.Citizen;

            if (citizen == null)
                return RedirectToHome();

            if (partyService.CanCandidateToCongress(citizen, citizen.PartyMember.Party) == false)
                return RedirectToHome();

            partyService.CandidateToCongress(citizen);
            AddInfo("You sucessfuly candidated to congress! <br/> Now you need to wait for approval from party president.");
            return RedirectToAction("View", new { partyID = citizen.PartyMember.PartyID});
        }

        private RedirectToRouteResult RedirectIfNotMember(int partyID)
        {
            AddMessage("You do not belong to this party!", PopupMessageType.Error);
            return RedirectToAction("View", new { partyID = partyID });
        }

        [HttpGet]
        [Route("Party/{partyID:int}/Manage")]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult Manage(int partyID)
        {
            var party = partyRepository.GetById(partyID);
            var citizen = SessionHelper.LoggedCitizen;
            var entity = SessionHelper.CurrentEntity;
            var partyMember = citizen.PartyMember;

            var partyRoles = partyRepository.GetPartyRoles();
            var joinMethods = partyRepository.GetJoinMethods();

            if (partyMember == null || partyMember.PartyID != partyID)
            {
                return RedirectIfNotMember(partyID);
            }

            var partyRole = (PartyRoleEnum)partyMember.PartyRoleID;

            if (partyRole < PartyRoleEnum.Manager)
            {
                AddError("You cannot manage this party!");
                return RedirectToAction("View", new { partyID = partyID });
            }

            var vm = new ManagePartyViewModel(party, partyRoles, joinMethods);

            return View(vm);
        }

        [HttpGet]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [IsCitizen]
        public ActionResult Create()
        {
            var citizen = SessionHelper.LoggedCitizen;
            var country = citizen.Region.Country;
            var countryPolicies = country.CountryPolicy;
            var countryMoney = country.Currency;
            var configuration = ConfigurationHelper.Configuration;

            if(citizen.PartyMember != null)
            {
                return YouAreAlreadyMemberOfThePartyRedirection();
            }

            var countryFee = new MoneyViewModel(countryMoney, countryPolicies.PartyFoundingFee);
            var adminFee = new MoneyViewModel(GameHelper.Gold, configuration.PartyFoundingFee);


            CreatePartyViewModel vm = new CreatePartyViewModel(adminFee, countryFee);

            return View(vm);
        }

        

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [IsCitizen]
        public ActionResult Create(CreatePartyViewModel vm)
        {
            var citizen = SessionHelper.LoggedCitizen;
            var country = citizen.Region.Country;
            var countryPolicies = country.CountryPolicy;
            var countryCurrency = country.Currency;
            var configuration = ConfigurationHelper.Configuration;

            var validator = new PartyCreateValidator(ModelState, entityService);


            var countryFee = new MoneyViewModel(countryCurrency, countryPolicies.PartyFoundingFee);
            var adminFee = new MoneyViewModel(GameHelper.Gold, configuration.PartyFoundingFee);

            if (validator.IsValid)
            {
                var party = partyService.CreateParty(vm.Name, citizen, country);

                var adminMoney = new Money()
                {
                    Amount = adminFee.Quantity,
                    Currency = GameHelper.Gold
                };

                var countryMoney = new Money()
                {
                    Amount = countryFee.Quantity,
                    Currency = countryCurrency
                };

                var adminTransaction = new Transaction()
                {
                    Arg1 = "Admin Fee",
                    Arg2 = string.Format("{0}({1}) created party {2}({3})", citizen.Entity.Name, citizen.Entity.EntityID, party.Entity.Name, party.ID),
                    DestinationEntityID = null,
                    Money = adminMoney,
                    SourceEntityID = citizen.Entity.EntityID,
                    TransactionType = TransactionTypeEnum.PartyCreate
                };
                var countryTransaction = new Transaction()
                {
                    Arg1 = "Country Fee",
                    Arg2 = adminTransaction.Arg2,
                    DestinationEntityID = country.ID,
                    Money = countryMoney,
                    SourceEntityID = citizen.Entity.EntityID,
                    TransactionType = TransactionTypeEnum.PartyCreate
                };

                transactionService.MakeTransaction(adminTransaction);
                transactionService.MakeTransaction(countryTransaction);

                AddMessage(new PopupMessageViewModel(string.Format("Party {0} was created!", party.Entity.Name)));
                return RedirectToAction("Index", "Home");
            }
            else
                RedirectBack();
            
            


            vm.CountryFee = countryFee;
            vm.AdminFee = adminFee;

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Party/{partyID:int}/Members")]
        public ActionResult Members(int partyID)
        {
            var party = partyRepository.GetById(partyID);
            if (party == null)
                return RedirectToHomeWithError("Party does not exist!");

            var members = partyRepository.GetMembers(partyID);
            var vm = new PartyMemberListViewModel(party, members);

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Party/{partyID:int}/Invites")]
        public ActionResult PartyInvites(int partyID)
        {
            var party = partyRepository.GetById(partyID);

            MethodResult result;
            if ((result = partyService.CanSeeInvites(SessionHelper.LoggedCitizen, party)).IsError)
                return RedirectBackWithError(result);

            var citizens = party.PartyInvites.Select(pm => pm.Citizen.Entity).ToList();

            var vm = new PartyPendingInviteListViewModel(party, citizens);

            return View(vm);

        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen)]
        public ActionResult SendJoinRequest(int partyID, string message)
        {
            var party = partyRepository.GetById(partyID);
            var citizen = SessionHelper.LoggedCitizen;

            MethodResult result;
            if ((result = partyService.CanSendJoinRequest(citizen, party, message)).IsError)
                return RedirectBackWithError(result);

            partyService.SendJoinRequest(citizen, party, message);
            return RedirectBackWithSuccess("You successfully sent join request.");
        }

        [HttpPost]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [EntityAuthorize(EntityTypeEnum.Citizen)]
        public ActionResult CancelJoinRequest(int requestID)
        {
            var request = partyJoinRequestRepository.GetById(requestID);
            var citizen = SessionHelper.LoggedCitizen;

            MethodResult result;
            if ((result = partyService.CanCancelJoinRequest(request, citizen)).IsError)
                return RedirectBackWithError(result);

            partyService.CancelJoinRequest(request);
            return RedirectBackWithSuccess("You successfully removed join request.");
        }

        [HttpPost]
        [AjaxOnly]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult DeclineJoinRequest(int requestID)
        {
            var request = partyJoinRequestRepository.GetById(requestID);
            var citizen = SessionHelper.LoggedCitizen;

            MethodResult result;
            if ((result = partyService.CanDeclineJoinRequest(request, citizen)).IsError)
                return JsonError(result);

            partyService.DeclineJoinRequest(request);

            return JsonSuccess("Request removed.");
        }


        [HttpPost]
        [AjaxOnly]
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        public ActionResult AcceptJoinRequest(int requestID)
        {
            var request = partyJoinRequestRepository.GetById(requestID);
            var citizen = SessionHelper.LoggedCitizen;

            MethodResult result;
            if ((result = partyService.CanAcceptJoinRequest(request, citizen)).IsError)
                return JsonError(result);

            //after accepting request entities will be deleted so we will need to access them right now.
            var citizenName = request.Citizen.Entity.Name;
            partyService.AcceptJoinRequest(request);
            
            return JsonSuccess($"Request accepted. {citizenName} is now your party member.");
        }

        private ActionResult YouAreAlreadyMemberOfThePartyRedirection()
        {
            AddError("You are already member of this party!");
            return RedirectToAction("Index", "Home");
        }
        private ActionResult PartyDoesNotExistRedirect()
        {
            AddError("Party does not exist!");
            return RedirectToAction("Index", "Home");
        }
    }
}