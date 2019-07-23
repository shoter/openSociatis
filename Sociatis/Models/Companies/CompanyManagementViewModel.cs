using Entities;
using Sociatis.Models.Entit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Companies
{
    public class CompanyManagementViewModel
    {
        public CompanyInfoViewModel Info { get; set; }

        public SmallEntityAvatarViewModel Founder { get; set; }
        public List<SmallEntityAvatarViewModel> Managers { get; set; } = new List<SmallEntityAvatarViewModel>();

        public CompanyManagementViewModel(Company company)
        {
            Info = new CompanyInfoViewModel(company);

            if (company.OwnerID.HasValue)
                Founder = new SmallEntityAvatarViewModel(company.Owner);

            foreach (var manager in company.CompanyManagers.ToList())
            {
                Managers.Add(new SmallEntityAvatarViewModel(manager.Entity));
            }


        }

    }
}