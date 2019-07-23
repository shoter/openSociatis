
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Extensions;
using Entities.enums;
using Entities;
using WebServices;
using Sociatis.Models.Navigation;
using Entities.Repository;
using Sociatis.Models.Citizens;

namespace Sociatis.Controllers
{
    public class NavigationController : ControllerBase
    {
        readonly ICompanyService companyService;
        readonly IConfigurationRepository configurationRepository;
        readonly ICountryRepository countryRepository;

        public NavigationController(ICountryRepository countryRepository, ICompanyService companyService, IConfigurationRepository configurationRepository,
            IPopupService popupService) : base(popupService)
        {
            this.countryRepository = countryRepository;
            this.companyService = companyService;
            this.configurationRepository = configurationRepository;
        }

        public ActionResult RenderNavigation()
        {
            var config = configurationRepository.GetConfiguration();

            var vm = new NavigationViewModel()
            {
                Day = config.CurrentDay
            };

            var urlHelper = new UrlHelper(Request.RequestContext);

            NavigationHelper.PrepareNavigation(ref vm, urlHelper);


            if (SessionHelper.CurrentEntity.GetCurrentCountry() != null)
            {
                vm.AddMarket(SessionHelper.CurrentEntity.GetCurrentCountry(), urlHelper);
                vm.AddCountry(SessionHelper.CurrentEntity.GetCurrentCountry(), urlHelper);
            }
            else
            {
                vm.AddMarket(countryRepository.First(), urlHelper);
            }
            if (SessionHelper.CurrentEntity?.Citizen?.PlayerTypeID == (int)PlayerTypeEnum.SuperAdmin)
            {
                addAdminSection(vm, urlHelper);
            }
            vm.AddMap(urlHelper);
            addOtherSection(vm, urlHelper);
            vm.AddRankings(urlHelper);
            /* vm.AddSociety();
             vm.AddRankings();
             vm.AddWars();*/

            return PartialView(vm);
        }

        private static void addOtherSection(NavigationViewModel vm, UrlHelper urlHelper)
        {
            NavigationSectionViewModel other = new NavigationSectionViewModel()
            {
                Name = "Other"
            };

            other.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Dev issues",
                Url = urlHelper.Action("Index", "DevIssue")
            });

            other.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Change password",
                Url = urlHelper.Action("ChangePassword", "Account")
            });


            var calcs = new NavigationSectionViewModel()
            {
                Name = "Calculators"
            };

            calcs.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Fuel cost calculator",
                Url = urlHelper.Action(nameof(CalculatorController.CalculateFuelCost), "Calculator")
            });

            calcs.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Production points calculator",
                Url = urlHelper.Action(nameof(CalculatorController.CalculateProductionPoints), "Calculator")
            });

            other.Children.Add(calcs);

            other.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Sociatis Wiki",
                Url = "http://wikisociatis.zzz.com.ua/wiki/"
            });

            other.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Logout",
                Url = urlHelper.Action("Logout", "Account")
            });

            vm.MainSections.Add(other);
        }

        private static void addAdminSection(NavigationViewModel vm, UrlHelper urlHelper)
        {
            NavigationSectionViewModel admin = new NavigationSectionViewModel()
            {
                Name = "Admin"
            };

            admin.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Edit map",
                Url = urlHelper.Action("Edit", "Map")
            });

            admin.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Edit country colors",
                Url = urlHelper.Action(nameof(AdminController.EditCountryColors), "Admin")
            });

            var debug = new NavigationSectionViewModel()
            {
                Name = "Debug"
            };

            admin.Children.Add(debug);

            debug.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Assign as Congressman",
                Url = urlHelper.Action("GiveMeCongressman", "Debug")
            });

            debug.Children.Add(new NavigationSectionViewModel()
            {
                Name = "Assign as President",
                Url = urlHelper.Action("GrantMePresident", "Debug")
            });

            debug.Children.Add(new NavigationSectionViewModel()
            {
                Name = "End congress voting",
                Url = urlHelper.Action("FinishCongressVoting", "Debug")
            });

            vm.MainSections.Add(admin);
        }



        //public ActionResult RenderBusiness()
        //{
        //    var entity = SessionHelper.CurrentEntity;
        //    switch(entity.GetEntityType())
        //    {
        //        case EntityTypeEnum.Citizen:
        //            {
        //                return CitizenBusiness(entity);
        //            }
        //        case EntityTypeEnum.Company:
        //            {
        //                return CompanyBusiness(entity);
        //            }
        //    }
        //    return Content("");
        //}
    }
}
