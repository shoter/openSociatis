using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Organisation
{
    public class OrganisationViewModel
    {
        public OrganisationInfoViewModel Info { get; set; }

        public OrganisationViewModel(Entities.Organisation organisation)
        {
            Info = new OrganisationInfoViewModel(organisation);
        }
    }
}