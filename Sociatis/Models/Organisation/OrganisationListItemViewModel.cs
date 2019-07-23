using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Organisation
{
    public class OrganisationListItemViewModel
    {
        public string OrganisationName { get; set; }
        public int OrganisationID { get; set; }
        public ImageViewModel Avatar { get; set; }

        public OrganisationListItemViewModel(Entities.Organisation organisation)
        {
            OrganisationName = organisation.Entity.Name;
            OrganisationID = organisation.ID;
            Avatar = new ImageViewModel(organisation.Entity.ImgUrl);
        }
    }
}