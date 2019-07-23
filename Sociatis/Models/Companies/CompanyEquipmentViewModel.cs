using Entities;
using Entities.Extensions;
using Sociatis.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Companies
{
    public class CompanyEquipmentViewModel : EquipmentViewModel
    {
        public CompanyInfoViewModel Info { get; set; }

        public CompanyEquipmentViewModel(Company company, Entities.Equipment equipment)
            :base(equipment)
        {
            Info = new CompanyInfoViewModel(company);
        }
    }
}