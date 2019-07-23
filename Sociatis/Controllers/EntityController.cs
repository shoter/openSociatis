using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entities.enums;
using Entities.Extensions;
using Sociatis.App_Start;
using Entities.Repository;
using WebServices;
using WebServices.BigParams.auth;
using Sociatis.Models.Companies;
using Sociatis.Models;
using Sociatis.Models.Citizens;
using Sociatis.Models.Party;
using Sociatis.Models.Organisation;
using Sociatis.Models.Country;
using Entities.structs;
using Sociatis.Models.Newspapers;
using Sociatis.ActionFilters;
using Entities;
using Sociatis.Code.Filters;
using Sociatis.Models.Hotels;

namespace Sociatis.Controllers
{
    
    public class EntityController : ControllerBase
    {
        ICitizenRepository citizensRepository;
        IAuthService authService;
        ICurrencyRepository currencyRepository;
        IEntityRepository entityRepository;
        IEntityService entityService;
        ICitizenService citizenService;
        ISummaryService summaryService;

        public EntityController(ICitizenRepository citizensRepository, IAuthService authService, ICurrencyRepository currencyRepository,
            IEntityRepository entityRepository, IEntityService entityService, ICitizenService citizenService, IPopupService popupService,
            ISummaryService summaryService) : base(popupService)
        {
            this.citizensRepository = citizensRepository;
            this.authService = authService;
            this.entityRepository = entityRepository;
            this.currencyRepository = currencyRepository;
            this.entityService = entityService;
            this.citizenService = citizenService;
            this.summaryService = summaryService;
        }
        // GET: Entity
        public ActionResult EntitySummary()
        {
            if (SessionHelper.Session == null)
                return Content("");

            switch(SessionHelper.CurrentEntity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    {
                        return CitizenSummary();
                    }
                case EntityTypeEnum.Company:
                    {
                        return CompanySummary();
                    }
                    case EntityTypeEnum.Party:
                    {
                        return PartySummary();
                    }
                case EntityTypeEnum.Organisation:
                    {
                        return OrganisationSummary();
                    }
                case EntityTypeEnum.Country:
                    {
                        return CountrySummary();
                    }
                case EntityTypeEnum.Newspaper:
                    {
                        return NewspaperSummary();
                    }
                case EntityTypeEnum.Hotel:
                    return HotelSummary();
            }

            return Content("Not found");
        }

        public ActionResult HotelSummary()
        {
            var entity = SessionHelper.CurrentEntity;

            var vm = new HotelSummaryViewModel(entity.Hotel);

            return PartialView("HotelSummary", vm);
        }

        [Route("Entity/{entityID:int}")]
        [DayChangeAuthorize]
        public RedirectToRouteResult View(int entityID)
        {
            var entity = entityRepository.GetById(entityID);
            var entityType = (EntityTypeEnum)entity.EntityTypeID;

            switch(entityType)
            {
                case EntityTypeEnum.Citizen:
                    {
                        return RedirectToAction("View", "Citizen", new { citizenID = entityID });
                    }
                case EntityTypeEnum.Company:
                    {
                        return RedirectToAction("View", "Company", new { companyID = entityID });
                    }
                case EntityTypeEnum.Party:
                    {
                        return RedirectToAction("View", "Party", new { partyID = entityID });
                    }
                case EntityTypeEnum.Organisation:
                    {
                        return RedirectToAction("View", "Organisation", new { organisationID = entityID });
                    }
                case EntityTypeEnum.Country:
                    {
                        return RedirectToAction("View", "Country", new { countryID = entityID });
                    }
                case EntityTypeEnum.Newspaper:
                    {
                        return RedirectToAction("View", "Newspaper", new { newspaperID = entityID });
                    }
                case EntityTypeEnum.Hotel:
                    {
                        return RedirectToAction("View", "Hotel", new { hotelID = entityID });
                    }
            }

            throw new NotImplementedException();
        }
        
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        public ActionResult Switch(int ID)
        {
            return Switch(ID, "Index", "Home");
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Switch(int ID, string actionName)
        {
            var entity = entityRepository.GetById(ID);
            if (hasSwitchedInto(entity) == false)
                return RedirectToAction("Index", "Home"); ;

            SwitchInto(entity);
            return RedirectToAction(actionName);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Switch(int ID, string actionName, string controllerName)
        {
            var entity = entityRepository.GetById(ID);

            if (hasSwitchedInto(entity) == false)
                return RedirectToAction("Index", "Home"); ;

            SwitchInto(entity);
            return RedirectToAction(actionName, controllerName);
        }

        public static void SwitchInto(Entity entity)
        {
            SessionHelper.SwitchStack.Push(new EntityDom(entity));
        }

        private bool hasSwitchedInto(Entity entity)
        {

            if (entityService.CanChangeInto(SessionHelper.CurrentEntity, entity, SessionHelper.LoggedCitizen) == false)
            {
                AddError("You cannot switch to this entity!");
                return false;
            }
           
            return true;
        }



        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        public ActionResult SwitchBack()
        {
            if (SessionHelper.SwitchStack.Count == 1)
                return RedirectBack();

            SessionHelper.SwitchStack.Pop();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CitizenSummary()
        {
            var entity = SessionHelper.CurrentEntity;
            var info = summaryService.GetCitizenSummaryInfo(entity.EntityID);

            var vm = new CitizenSummaryViewModel(info, citizenService);

            return PartialView("CitizenSummary", vm);
        }
        public ActionResult NewspaperSummary()
        {
            var entity = SessionHelper.CurrentEntity;
            var newspaper = entity.Newspaper;

            var vm = new NewspaperSummaryViewModel(newspaper);

            return PartialView("NewspaperSummary", vm);
        }

        public ActionResult CompanySummary()
        {
            var entity = SessionHelper.CurrentEntity;
            var company = entity.Company;
            var session = SessionHelper.Session;

            var vm = new CompanySummaryViewModel(company);

            return PartialView("CompanySummary", vm);
        }

        public ActionResult PartySummary()
        {
            var entity = SessionHelper.CurrentEntity;
            var party = entity.Party;

            var citizen = SessionHelper.LoggedCitizen;
            var userPartyRole = (PartyRoleEnum)citizen.PartyMember.PartyRoleID;

            var vm = new PartySummaryViewModel(party, SessionHelper.Session);

            return PartialView("PartySummary", vm);
        }

        public ActionResult OrganisationSummary()
        {
            var entity = SessionHelper.CurrentEntity;
            var organisation = entity.Organisation;


            var vm = new OrganisationSummaryViewModel(organisation);

            return PartialView("OrganisationSummary", vm);
        }

        public ActionResult CountrySummary()
        {
            var entity = SessionHelper.CurrentEntity;
            var country = entity.Country;

            var vm = new CountrySummaryViewModel(country);

            return PartialView("CountrySummary", vm);
        }

    }
}