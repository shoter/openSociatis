using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Organisation
{
    public class OrganisationListViewModel
    {
        public List<ShortEntityInfoViewModel> Organisations { get; set; } = new List<ShortEntityInfoViewModel>();

        public OrganisationListViewModel(IEnumerable<Entities.Organisation> organisations)
        {
            foreach(var organisation in organisations)
            {
                Organisations.Add(new ShortEntityInfoViewModel(organisation.Entity));
            }
        }
    }
}