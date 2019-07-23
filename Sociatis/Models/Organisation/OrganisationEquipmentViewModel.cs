using Sociatis.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Organisation
{
    public class OrganisationEquipmentViewModel : EquipmentViewModel
    {
        public OrganisationInfoViewModel Info { get; set; }

        public OrganisationEquipmentViewModel(Entities.Organisation company, Entities.Equipment equipment)
            :base(equipment)
        {
            Info = new OrganisationInfoViewModel(company);
        }
    }
}