using Entities;
using Entities.enums;
using Entities.structs.Newspapers;
using Entities.Extensions;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using Entities.Repository;
using System.Web.Mvc;
using Sociatis.Models.Infos;
using Sociatis.Models.Avatar;

namespace Sociatis.Models.Newspapers
{
    public class NewspaperInfoViewModel : ShortEntityInfoViewModel
    {
        public string CountryName { get; set; }
        public int CountryID { get; set; }
        public NewspaperRightsEnum NewspaperRights { get; set; }
        public int PaperCount { get; set; }
        public int InventoryCapacity { get; set; }

        public int OwnerID { get; set; }
        public string OwnerName { get; set; }

        public bool CanSwitch { get; set; }

        public InfoMenuViewModel Menu { get; set; }

        public AvatarChangeViewModel AvatarChange{ get; set; }



        public NewspaperInfoViewModel(Newspaper newspaper, INewspaperService newspaperService) : base(newspaper.Entity)
        {
            CountryName = newspaper.Country.Entity.Name;
            CountryID = newspaper.CountryID;

            NewspaperRights = newspaperService.GetNewspaperRights(newspaper, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            var productRepository = DependencyResolver.Current.GetService<IProductRepository>();

            InventoryCapacity = newspaper.Entity.Equipment.ItemCapacity;
            PaperCount = newspaper.Entity.GetEquipmentItem(ProductTypeEnum.Paper, 1, productRepository).Amount;

            var owner = newspaper.Owner;

            OwnerID = owner.EntityID;
            OwnerName = owner.Name;

            CanSwitch = NewspaperRights == NewspaperRightsEnum.Full;

            createMenu();

            AvatarChange = new AvatarChangeViewModel(newspaper.ID);
        }

        private void createMenu()
        {
            Menu = new InfoMenuViewModel();
            if (CanSwitch)
            {

                Menu.AddItem(InfoActionViewModel.CreateEntitySwitch(EntityID))
                    .AddItem(new InfoActionViewModel("ChangeOwnership", "Newspaper", "Change ownership", "fa-pencil", new { newspaperID = EntityID }));
            }

            
            if (NewspaperRights > 0)
            {
                var exp = new InfoExpandableViewModel("Actions", "fa-ellipsis-h");
                Menu.AddItem(exp);

                if (NewspaperRights.HasFlag(NewspaperRightsEnum.CanWriteArticles))
                {
                    exp.AddChild(new InfoActionViewModel("WriteArticle", "Newspaper", "Create article", "fa-pencil-square-o", FormMethod.Get, new { newspaperID = EntityID }));
                }

                if (NewspaperRights.HasFlag(NewspaperRightsEnum.CanManageArticles))
                {
                    exp.AddChild(new InfoActionViewModel("ManageArticles", "Newspaper", "Manage articles", "fa-newspaper-o", FormMethod.Get, new { newspaperID = EntityID }));
                }

                if (NewspaperRights.HasFlag(NewspaperRightsEnum.CanManageJournalists))
                {
                    exp.AddChild(new InfoActionViewModel("ManageJournalists", "Newspaper", "Manage journalists", "fa-users", FormMethod.Get, new { newspaperID = EntityID }));
                }
            }

            if (SessionHelper.CurrentEntity.EntityID != EntityID)
                Menu.AddItem(InfoExpandableViewModel.CreateExchange(EntityID));

            


        }
    }
}