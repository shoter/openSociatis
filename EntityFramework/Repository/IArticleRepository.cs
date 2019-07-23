using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IArticleRepository : IRepository<Article>
    {
        void AddComment(ArticleComment comment);
        void AddBuyer(int articleID, int entityID);
        bool isBuyer(int articleID, int entityID);
        void AddVote(int articleID, int entityID, int score);
        ArticleVote GetVote(int articleID, int entityID);
        void Remove(ArticleVote vote);
    }
}
