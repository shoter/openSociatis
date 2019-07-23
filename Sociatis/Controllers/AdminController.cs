using Entities;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class AdminController : ControllerBase
    {
        ITransactionLogRepository logRepository;
        ICountryRepository countryRepository;

        public AdminController(ITransactionLogRepository logRepository, ICountryRepository countryRepository, IPopupService popupService) : base(popupService)
        {
            this.logRepository = logRepository;
            this.countryRepository = countryRepository;
        }
        // GET: Admin
        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult TransactionLogs()
        {
            List<TransactionLog> logs = new List<TransactionLog>();
            logs.AddRange(logRepository.Where(l => true).OrderBy(l => l.ID).Take(1000));

            return View(logs);
        }

        [SociatisAuthorize(PlayerTypeEnum.Admin)]
        public ActionResult CountryPolciies()
        {
            var countries = countryRepository.GetAll();
            var vm = new List<CountryPolicyViewModel>();

            foreach(var country in countries)
            {
                var policy = country.CountryPolicy;

                vm.Add(new CountryPolicyViewModel(policy));
            }

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.SuperAdmin)]
        [HttpGet]
        public ActionResult EditCountryColors()
        {
            var countries = countryRepository.GetAll();

            var vm = countries.Select(country =>
           new CountryColorViewModel(country)).ToList();

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.SuperAdmin)]
        [HttpPost]
        public ActionResult EditCountryColors(List<CountryColorViewModel> vm)
        {
            var countries = countryRepository.GetAll();

            foreach (var country in vm)
            {
                var dbCountry = countries.First(c => c.ID == country.CountryID);
                dbCountry.Color = country.CountryColor;
                country.CountryName = dbCountry.Entity.Name;
            }

            countryRepository.SaveChanges();

            return View(vm);
        }
    }
}