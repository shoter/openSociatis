using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.structs;

namespace Sociatis.Models.Companies.Managers
{
    public class ManagerViewModel
    {
        public SmallEntityAvatarViewModel Avatar { get; set; }
        public string Title { get; set; }
        public CompanyRights Rights { get; set; }
        public bool ReadOnly { get; set; } = true;
        public int? ManagerID { get; set; }

        public ManagerViewModel(CompanyManager manager)
        {
            var entity = manager.Entity;

            Avatar = new SmallEntityAvatarViewModel(entity);
            Title = "Manager";
            Rights = new CompanyRights(manager);
            ManagerID = manager.ID;
        }

        public ManagerViewModel(Entity companyOwner)
        {
            Avatar = new SmallEntityAvatarViewModel(companyOwner);
            Title = "CEO";
            Rights = new CompanyRights(fullRights: true);
        }
    }
}