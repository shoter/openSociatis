using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Newspapers
{
    public class NewspaperIndexViewModel
    {
        public List<NewspaperInfoViewModel> Newspapers { get; set; } = new List<NewspaperInfoViewModel>();
        public NewspaperIndexViewModel(List<Newspaper> newspapers, INewspaperService newspaperService)
        {
            foreach (var newspaper in newspapers)
                Newspapers.Add(new NewspaperInfoViewModel(newspaper, newspaperService));
        }
    }
}