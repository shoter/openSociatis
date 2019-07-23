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
    public class ChangeNewspaperOwnerViewModel
    {
        public NewspaperInfoViewModel Info { get; set; }
        public int NewOwnerID { get; set; }
        public Select2AjaxViewModel OwnerSelect { get; set; }

        public ChangeNewspaperOwnerViewModel()
        {
            initSelect2();
        }

        public ChangeNewspaperOwnerViewModel(Newspaper newspaper, INewspaperService newspaperService) : this()
        {
            Info = new NewspaperInfoViewModel(newspaper, newspaperService);
        }

        private void initSelect2()
        {
            OwnerSelect = Select2AjaxViewModel.Create<NewspaperController>(c => c.GetEligibleEntitiesForOwnershipChange(null), "NewOwnerID", null, "");
        }
    }
}