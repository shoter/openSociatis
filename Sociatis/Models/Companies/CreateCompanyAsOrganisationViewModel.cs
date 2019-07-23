using Entities.Repository;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Companies
{
    public class CreateCompanyAsOrganisationViewModel : CreateCompanyViewModel
    {
        public List<SelectListItem> Regions { get; set; }

        public void LoadSelectList(IRegionRepository regionRepository)
        {
            Regions = regionRepository.Where(r => r.CountryID == SessionHelper.CurrentEntity.Organisation.CountryID)
                .ToList()
                .Select(r => new SelectListItem() { Value = r.ID.ToString(), Text = r.Name })
                .ToList();
        }
    }
}