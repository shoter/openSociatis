using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Newspapers
{
    public class ViewNewspaperViewModel
    {
        public NewspaperInfoViewModel Info { get; set; }
        public List<ArticleShortViewModel> LastArticles { get; set; } = new List<ArticleShortViewModel>();

        public ViewNewspaperViewModel(Newspaper newspaper, INewspaperService newspaperService)
        {
            Info = new NewspaperInfoViewModel(newspaper, newspaperService);
            foreach(var article in newspaper.Articles
                .OrderByDescending(a => a.ID)
                .Where(a => a.Published)
                .Take(10)
                .ToList())
            {
                LastArticles.Add(new ArticleShortViewModel(article));
            }
        }
    }
}