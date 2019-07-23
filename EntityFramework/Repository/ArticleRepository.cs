using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ArticleRepository : RepositoryBase<Article, SociatisEntities>, IArticleRepository
    {
        public ArticleRepository(SociatisEntities context) : base(context)
        {
        }

        public void AddComment(ArticleComment comment)
        {
            context.ArticleComments
                .Add(comment);
        }

        public void AddBuyer(int articleID, int entityID)
        {
            var article = GetById(articleID);
            var entity = context.Entities.First(e => e.EntityID == entityID);

            article.Buyers.Add(entity);
        }

        public void AddVote(int articleID, int entityID, int score)
        {
            var vote = new ArticleVote()
            {
                ArticleID = articleID,
                EntityID = entityID,
                Score = score
            };

            context.ArticleVotes.Add(vote);
        }

        public ArticleVote GetVote(int articleID, int entityID)
        {
            return context.ArticleVotes
                .FirstOrDefault(a => a.ArticleID == articleID && a.EntityID == entityID);
        }

        public bool isBuyer(int articleID, int entityID)
        {
            var article = GetById(articleID);

            return article.Buyers
                .Select(b => b.EntityID)
                .Contains(entityID);
        }

        public void Remove(ArticleVote vote)
        {
            context.ArticleVotes.Remove(vote);
        }
    }
}
