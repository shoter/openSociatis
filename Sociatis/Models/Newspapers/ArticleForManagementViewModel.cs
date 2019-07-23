using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Newspapers
{
    public class ArticleForManagementViewModel
    {
        public int ID { get; set; }
        public string AuthorName { get; set; }
        public int? AuthorID { get; set; }
        public string Title { get; set; }
        public double? Price { get; set; }
        public bool IsPublished { get; set; }
        public bool HasPaidContent { get; set; }
        public string CreationDate { get; set; }
        public double Income { get; set; }

        public ArticleForManagementViewModel() { }
        public ArticleForManagementViewModel(Article article)
        {
            ID = article.ID;
            Title = article.Title;
            Price = (double?)article.Price;
            AuthorName = article.Author?.Name ?? "";
            AuthorID = article.AuthorID;
            CreationDate = string.Format("Day {0} {1}", article.CreationDay, article.CreationDate.ToShortTimeString());
            IsPublished = article.Published;
            HasPaidContent = Price.HasValue;
            Income = (double)article.GeneratedIncome;

        }
    }
}