using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebUtils;

namespace Sociatis.Models.Newspapers
{
    public class ManageArticlesViewModel
    {
        public NewspaperInfoViewModel Info { get; set; }
        public List<ArticleForManagementViewModel> Articles { get; set; } = new List<ArticleForManagementViewModel>();
        public PagingParam PagingParam { get; set; }

        public ManageArticlesViewModel(Newspaper newspaper, IList<Article> articles, INewspaperService newspaperService, PagingParam pagingParam)
        {
            Info = new NewspaperInfoViewModel(newspaper, newspaperService);

            foreach (var article in articles)
                Articles.Add(new ArticleForManagementViewModel(article));

            PagingParam = pagingParam;
        }
    }
}