using Entities;
using Sociatis.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebUtils.Forms.Select2;

namespace Sociatis.Models.Newspapers
{
    public class ManageJournalistsViewModel
    {
        public NewspaperInfoViewModel Info { get; set; }
        public List<JournalistForManageViewModel> Journalists { get; set; } = new List<JournalistForManageViewModel>();
        public Select2AjaxViewModel CitizenSelector { get; set; }

        public ManageJournalistsViewModel(Newspaper newspaper, INewspaperService newspaperService) : this()
        {
            Info = new NewspaperInfoViewModel(newspaper, newspaperService);

            foreach (var journalist in newspaper.NewspaperJournalists.ToList())
            {
                Journalists.Add(new JournalistForManageViewModel(journalist));
            }
        }

        public ManageJournalistsViewModel()
        {
            CitizenSelector = Select2AjaxViewModel.Create<CitizenController>(c => c.GetCitizens(null), "citizenID", null, "");
        }
    }
}