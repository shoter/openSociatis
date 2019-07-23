using Common;
using Common.EncoDeco;
using Entities;
using Entities.enums;
using Entities.Repository;
using Entities.structs;
using ObjectDumper;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models;
using Sociatis.Models.Account;
using Sociatis.Validators;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs;

namespace Sociatis.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly ICitizenRepository citizensRepository;
        private readonly ICountryRepository countriesRepository;
        private readonly IRegionRepository regionsRepository;
        private readonly ISessionRepository sessionRepository;
        private readonly IRegionService regionService;
        private readonly IEntityService entityService;

        public AccountController(IAuthService authService, ICitizenRepository citizensRepository, ICountryRepository countriesRepository,
            IRegionRepository regionsRepository, ISessionRepository sessionRepository, IPopupService popupService, IRegionService regionService,
            IEntityService entityService) : base(popupService)
        {
            this.authService = authService;
            this.citizensRepository = citizensRepository;
            this.countriesRepository = countriesRepository;
            this.regionsRepository = regionsRepository;
            this.sessionRepository = sessionRepository;
            this.regionService = regionService;
            this.entityService = entityService;
        }


        // GET: Account
        public ActionResult Login()
        {
            var vm = new LoginRegisterViewModel();
            vm.Register.LoadSelectLists(countriesRepository, regionService);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Login(LoginRegisterViewModel VM)
        {
            if (VM.Login.Name != null)
            {
                var vm = VM.Login;
                try
                {
                    AccountValidator validator = new AccountValidator(ModelState, citizensRepository, entityService);
                    validator.ModelStatePrefix = "Login.";
                    validator.Validate(vm);

                    if (validator.IsValid)
                    {
                        SessionHelper.SwitchStack = new Stack<EntityDom>();
                        Credentials credentials = createCredentials(vm);
                        //now we will delete old cookies associated with the users. They are not needed
                        deleteOldSessions(credentials.Username);

                        SessionHelper.Session = authService.Login(credentials, SessionHelper.ClientIP);
                        SessionHelper.SwitchStack.Push(new EntityDom(SessionHelper.LoggedCitizen.Entity));

                        return RedirectToAction("Index", "Home");

                    }
                    VM.Login = vm;
                    VM.Register = new RegisterViewModel();
                    VM.Register.LoadSelectLists(countriesRepository, regionService);
                    return View(VM);
                }
                catch (DbEntityValidationException e)
                {
                    var str = "";

                    foreach (var error in e.EntityValidationErrors)
                    {

                        str += string.Format("Name : {0}<br/>", error.Entry.Entity.GetType().Name);
                        foreach (var ee in error.ValidationErrors)
                        {
                            str += string.Format("{0} - {1}<br/>", ee.PropertyName, ee.ErrorMessage);
                        }
                        str += "<br/>";
                    }

                    str = string.Format("<html><body>{0}</body></html>", str);
                    return Content(str);
                }
            }
            else
            {
                var vm = VM.Register;
                vm.LoadSelectLists(countriesRepository, regionService);
                AccountValidator validator = new AccountValidator(ModelState, citizensRepository, entityService);
                validator.ModelStatePrefix = "Register.";
                validator.Validate(vm);

                if (validator.IsValid)
                {

                    RegisterInfo info = new RegisterInfo()
                    {
                        Name = vm.Name,
                        Password = vm.Password,
                        CountryID = vm.CountryID.Value,
                        RegionID = vm.RegionID.Value,
                        Email = vm.Email,
                        PlayerType = PlayerTypeEnum.Player
                    };

                    AddSuccess("Your account was created!");
                    authService.Register(info);

                    return RedirectToAction("Login");
                }
                VM.Register = vm;
                return View(VM);
            }

        }

        public ActionResult ForgotPassword(string email)
        {
            authService.ForgotPassword(email);
            AddSuccess("If you provided correct address then you should have email with new random password on your inbox.");
            return RedirectToAction(nameof(Login));
        }

        public ActionResult LoggedCitizen()
        {
            Citizen vm = SessionHelper.LoggedCitizen;

            return PartialView(vm);
        }

        private Credentials createCredentials(LoginViewModel vm)
        {
            return new Credentials()
            {
                Password = vm.Password,
                RememberMe = vm.RememberMe,
                Username = vm.Name
            };
        }

        private void deleteOldSessions(string username)
        {
            sessionRepository
                                 .Where(s => s.Citizen.Entity.Name == username)
                                 .ToList()
                                 .ForEach(s => sessionRepository.Remove(s.ID));
            sessionRepository.SaveChanges();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Account/ChangePassword")]
        public ActionResult ChangePassword()
        {
            var vm = new ChangePasswordViewModel();
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        [Route("Account/ChangePassword")]
        public ActionResult ChangePassword(ChangePasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var citizen = citizensRepository.GetByCredentials(SessionHelper.LoggedCitizen.Entity.Name, vm.OldPassword);

                var result = authService.CanChangePassword(citizen, vm.OldPassword, vm.NewPassword);
                if (result.IsError)
                    AddError(result);
                else
                {
                    authService.ChangePassword(citizen, vm.NewPassword);
                    AddSuccess("Your password was changed!");
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpGet]
        [Route("Account/Logout")]
        public ActionResult Logout()
        {
            authService.Logoff(SessionHelper.LoggedCitizen);
            SessionHelper.WipeSession();
            return RedirectToAction(nameof(Login));
        }

        public ActionResult test()
        {
            string _out = SessionHelper.Session.DumpToString("Session");
            _out = _out.Replace(Environment.NewLine, "</br>");
            
            return View(_out as object);
        }

        public ActionResult Register()
        {
            RegisterViewModel vm = new RegisterViewModel();
            vm.LoadSelectLists(countriesRepository, regionService);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel vm)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult GetStartingRegions(int countryID)
        {
            var regions = regionService.GetBirthableRegions(countryID)
                .Select(r => new SelectListItem() {

                    Text = r.CountryCoreID == r.CountryID ? r.Name : r.Name + "(occupied)"
                
                , Value = r.ID.ToString() }).ToList();

            return PartialView("_DropDownListItems", regions);
        }
    }
}