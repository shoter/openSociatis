using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Newspapers
{
    public class ArticleShortViewModel
    {
        public ImageViewModel Image { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string ShortContent { get; set; }
        public int ID { get; set; }

        public ArticleShortViewModel(Article article)
        {
            ID = article.ID;
            Image = new ImageViewModel(article.ImgURL);
            Title = article.Title;
            Date = string.Format("day {0} {1}", article.CreationDay, article.CreationDate.ToShortTimeString());
            ShortContent = article.ShortDescription + " ...";
        }
    }
}