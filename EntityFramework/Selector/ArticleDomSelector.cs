using Common.EntityFramework;
using Entities.structs.Newspapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUtils;
using WebUtils.Extensions;

namespace Entities.Selector
{
    public class ArticleDomSelector : IDomSelector<Article, ArticleDom>
    {
        private PagingParam pp;
        private readonly bool includeComments;
        private readonly int? buyerID;

        public ArticleDomSelector(PagingParam pagingParam, int? buyerID = null, bool IncludeComments = true)
        {
            this.pp = pagingParam;
            includeComments = IncludeComments;
            this.buyerID = buyerID;
        }

        public IQueryable<ArticleDom> Select(IQueryable<Article> query)
        {
            if (includeComments)
                return selectWithComments(query);

            return baseSelect(query);
        }

        protected IQueryable<ArticleDom> baseSelect(IQueryable<Article> query)
        {
            return query
                .Select(a =>
            new ArticleDom()
            {
                ArticleID = a.ID,
                Content = a.Content,
                CreationDate = a.CreationDate,
                ImgURL = a.ImgURL,
                PaidContent = a.PayOnlyContent,
                Title = a.Title,
                VoteScore = a.VoteScore,
                CreationDay = a.CreationDay,
                Price = (double?)a.Price,
                ShortDescription = a.ShortDescription,
                NewspaperID = a.NewspaperID,
                CommentCount = a.ArticleComments.Count(),
                CountryID = a.Newspaper.CountryID,
                UnlockedContent = a.Buyers.Any(b => b.EntityID == buyerID),
                Published = a.Published,
            });
        }

        protected IQueryable<ArticleDom> selectWithComments(IQueryable<Article> query)
        {
            int skipCommentAmount = (pp.PageNumber - 1) * pp.RecordsPerPage;

            return query
                .Select(a =>
            new ArticleDom()
            {
                ArticleID = a.ID,
                Content = a.Content,
                CreationDate = a.CreationDate,
                ImgURL = a.ImgURL,
                PaidContent = a.PayOnlyContent,
                Title = a.Title,
                VoteScore = a.VoteScore,
                CreationDay = a.CreationDay,
                Price = (double?)a.Price,
                ShortDescription = a.ShortDescription,
                NewspaperID = a.NewspaperID,
                CountryID = a.Newspaper.CountryID,
                UnlockedContent = a.Buyers.Any(b => b.EntityID == buyerID),
                Published = a.Published,

                CommentCount = a.ArticleComments.Count(),
                Comments = a.ArticleComments
                .OrderByDescending(c => c.ID)
                .Skip(skipCommentAmount)
                .Take(pp.RecordsPerPage)
                .Select(c => new ArticleCommentDom()
                {
                    AuthorID = c.AuthorID,
                    AuthorImgURL = c.Author.ImgUrl,
                    AuthorName = c.Author.Name,
                    Content = c.Content,
                    CreationDate = c.CreationDate,
                    CreationDay = c.CreationDay,
                    EntityTypeID = c.Author.EntityTypeID
                }).ToList()
            });
        }
    }
}
