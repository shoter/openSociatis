using Entities.enums;
using Sociatis.Helpers;
using Sociatis.Models.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Country
{
    public class CountryInfoViewModel
    {
        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public ImageViewModel CountryImage { get; set; }
        public bool IsPresident { get; set; }
        public bool CanSeeTreasury { get; set; }
        public bool CanConstructCompany { get; set; }

        public InfoMenuViewModel Menu { get; set; } = new InfoMenuViewModel();
        public CountryInfoViewModel(Entities.Country country)
        {
            CountryImage = new ImageViewModel(Images.GetCountryFlag(country.Entity.Name));
            CountryName = country.Entity.Name;
            CountryID = country.ID;

            var countrySerivce = DependencyResolver.Current.GetService<ICountryService>();

            IsPresident = countrySerivce.IsPresident(country, SessionHelper.CurrentEntity);

            var countryTreasuryService = DependencyResolver.Current.GetService<ICountryTreasureService>();
            CanSeeTreasury = countryTreasuryService.CanSeeCountryTreasure(country, SessionHelper.CurrentEntity).isSuccess;

            var policy = country.CountryPolicy;

            CanConstructCompany = policy.CountryCompanyBuildLawAllowHolder != (int)LawAllowHolderEnum.Congress;

            createMenu();
        }

        private void createMenu()
        {
            if (IsPresident)
                createPresidentMenu();

            if (CanSeeTreasury)
                Menu.AddItem(new InfoActionViewModel("Treasury", "Country", "Treasury", "fa-dollar", new { countryID = CountryID }));

            Menu.AddItem(new InfoExpandableViewModel("Country Info", "fa-ellipsis-h")
                .AddChild(new InfoActionViewModel("President", "Country", "President", "fa-ellipsis-h", new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("PresidentCandidates", "Country", "President Candidates", "fa-ellipsis-h", new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("Geography", "Country", "Geography", "fa-ellipsis-h", new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("Wars", "Country", "Wars", "fa-ellipsis-h", new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("MPPs", "Country", "MPPs", "fa-file", new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("Embargoes", "Country", "Embargoes", "fa-ellipsis-h", new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("Parties", "Party", "Parties", "fa-users", new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("NationalConstructions", "Country", "National constructions", "fa-cog", new { countryID = CountryID }))
                
                );

            if (SessionHelper.CurrentEntity.EntityID != CountryID)
                Menu.AddItem(InfoExpandableViewModel.CreateExchange(CountryID));
        }

        private void createPresidentMenu()
        {
            var exp = new InfoExpandableViewModel("President", "fa-ellipsis-h");

            exp.AddChild(InfoActionViewModel.CreateEntitySwitch(CountryID))
                .AddChild(new InfoActionViewModel("DeclareWar", "Country", "Declare War", "fa-fighter-jet", FormMethod.Get, new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("DeclareEmbargo", "Country", "Declare Embargo", "fa-exclamation-circle", FormMethod.Get, new { countryID = CountryID }))
                .AddChild(new InfoActionViewModel("SendMPPOffer", "Country", "Send MPP Offer", "fa-file", FormMethod.Get, new { countryID = CountryID }));

            if (CanConstructCompany)
                exp.AddChild( new InfoActionViewModel("ConstructCompany", "Country", "Construct Company", "fa-heart", FormMethod.Get, new { countryID = CountryID }));

            Menu.AddItem(exp);
        }
    }
}