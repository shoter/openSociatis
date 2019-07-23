using Entities.structs.Newspapers;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.Helpers;

namespace Sociatis.Models.Home
{
    public class HomeArticleSpotlightViewModel
    {
        public int ID { get; set; }
        public ImageViewModel CountryFlag { get; set; }
        public ImageViewModel ArticleImage { get; set; }
        public string Title { get; set; }
        public string Score { get; set; }
        public int Day { get; set; }

        public HomeArticleSpotlightViewModel(ArticleDom article)
        {
            ID = article.ArticleID;
            Day = article.CreationDay;
            CountryFlag = Images.GetCountryFlag(article.CountryID).VM;
            ArticleImage = new ImageViewModel(article.ImgURL);
            Title = article.Title;
            Score = ScoreHelper.ToString(article.VoteScore);
        }
    }
}