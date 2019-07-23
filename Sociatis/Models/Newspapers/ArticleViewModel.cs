using Entities;
using Entities.Extensions;
using Entities.structs.Newspapers;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;
using WebUtils;

namespace Sociatis.Models.Newspapers
{
    public class ArticleViewModel
    {
        public int ID { get; set; }
        public NewspaperInfoViewModel Info { get; set; }
        public string Title { get; set; }
        public ImageViewModel Image { get; set; }
        public string Content { get; set; }
        public bool HasPayContent { get; set; }
        public string PayOnlyContent { get; set; }
        public double? Price { get; set; }
        public string CurrencySymbol { get; set; }
        public bool CanSeePayContent { get; set; }
        public string Score { get; set; }
        public int? VoteScore { get; set; }

        public bool CanVote { get; set; }

        public PagingParam PagingParam { get; set; }

        public List<ArticleCommentViewModel> Comments { get; set; } = new List<ArticleCommentViewModel>();

        public ArticleViewModel(ArticleDom article, Newspaper newspaper, INewspaperService newspaperService, PagingParam pagingParam )
        {
            Info = new NewspaperInfoViewModel(newspaper, newspaperService);
            CurrencySymbol = Persistent.Countries.GetCountryCurrency(newspaper.CountryID).Symbol;

            ID = article.ArticleID;
            Score = parseScore(article.VoteScore);
            Title = article.Title;
            Content = article.Content;
            HasPayContent = article.Price.HasValue;
            PayOnlyContent = article.PaidContent;
            Price = (double?)article.Price;
            CanSeePayContent = article.UnlockedContent;
            CanVote = SessionHelper.CurrentEntity.GetEntityType() == Entities.enums.EntityTypeEnum.Citizen;

            foreach(var comment in article.Comments.OrderBy(comment => comment.CreationDate))
            {
                Comments.Add(new ArticleCommentViewModel(comment));
            }

            PagingParam = pagingParam;

            VoteScore = newspaperService.GetVoteScore(ID, SessionHelper.CurrentEntity);

            if(Price != null)
            {
                var policy = newspaper.Country.CountryPolicy;
                var tax = 1 + (double)policy.ArticleTax;

                Price *= tax;
            }
        }

        private string parseScore(int voteScore)
        {
            if (voteScore < 1000)
                return voteScore.ToString();
            else
            {
                voteScore /= 1000;
                return voteScore.ToString() + "K";
            }
        }
    }
}