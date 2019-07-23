using Sociatis.Helpers;
using Sociatis.Models;
using Sociatis.Models.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs.Organisations;

namespace Sociatis.Models.Organisation
{
    public class OrganisationInfoViewModel
    {
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public string OwnerName { get; set; }
        public int? OwnerID { get; set; }
        public ImageViewModel Avatar { get; set; }
        public bool IsUnderControl { get; set; } = false;
        public OrganisationRights Rights { get; set; } = new OrganisationRights(false);

        public InfoMenuViewModel Menu { get; set; } = new InfoMenuViewModel();

        public OrganisationInfoViewModel(Entities.Organisation organisation)
        {
            OrganisationID = organisation.ID;
            OrganisationName = organisation.Entity.Name;
            CountryName = organisation.Country.Entity.Name;
            CountryID = organisation.CountryID;
            OwnerID = organisation.OwnerID;
            OwnerName = organisation.Owner?.Name ?? "";
            Avatar = new ImageViewModel(organisation.Entity.ImgUrl);

            if(organisation.OwnerID == SessionHelper.CurrentEntity.EntityID)
            {
                IsUnderControl = true;
            }

            var organisationService = DependencyResolver.Current.GetService<IOrganisationService>();
            Rights = organisationService.GetOrganisationRights(SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen, organisation);

            createMenu();
        }

        private void createMenu()
        {
            if (Rights.AnyRights)
                createManageMenu();
            if (SessionHelper.CurrentEntity.EntityID != OrganisationID)
                Menu.AddItem(InfoExpandableViewModel.CreateExchange(OrganisationID));
        }

        private void createManageMenu()
        {
            if (Rights.CanSwitchInto)
                Menu.AddItem(InfoActionViewModel.CreateEntitySwitch(OrganisationID));
            if (Rights.CanSeeInventory)
                Menu.AddItem(new InfoActionViewModel("Inventory", "Organisation", "Inventory", "fa-cubes", new { organisationID = OrganisationID }));
            if (Rights.CanSeeWallet)
                Menu.AddItem(new InfoActionViewModel("Wallet", "Organisation", "Wallet", "fa-dollar", new { organisationID = OrganisationID }));
        }
    }
}