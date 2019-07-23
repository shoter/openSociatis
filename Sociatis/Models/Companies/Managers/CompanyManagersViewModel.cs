using Entities;
using Microsoft.Ajax.Utilities;
using Sociatis.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.structs;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Companies.Managers
{
    public class CompanyManagersViewModel
    {
        public List<ManagerViewModel> Managers { get; set; } = new List<ManagerViewModel>();
        public Select2AjaxViewModel NewManagersSelector { get; set; }
        public CompanyInfoViewModel Info { get; set; }

        public CompanyManagersViewModel(Company company, CompanyRights currentEntityRights)
        {
            Info = new CompanyInfoViewModel(company);
            NewManagersSelector = Select2AjaxViewModel.Create<CompanyController>(c => c.GetAppropriateManagers(null), "entityID", null, "");

            initManagers(company, currentEntityRights);
        }

        private void initManagers(Company company, CompanyRights currentEntityRights)
        {
            Managers.AddRange(
                company.CompanyManagers.ToList().Select(manager => new ManagerViewModel(manager))
                );

            Managers.Add(new ManagerViewModel(company.Owner));

            Managers = Managers.OrderByDescending(m => m.Rights.Priority).ToList();

            if (currentEntityRights.CanManageManagers)
            {
                Managers.Where(m => m.Rights.Priority < currentEntityRights.Priority)
                    .ForEach(m => m.ReadOnly = false);
            }
        }
    }
}