using Common.Operations;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebServices.structs;

namespace WebServices
{
    public interface INewspaperService
    {
        /// <summary>
        /// Without checking the gold etc.
        /// </summary>
        MethodResult CanCreateNewspaper(Entity entity);
        /// <summary>
        /// With checking gold amount, national currency amount, is name good
        /// </summary>
        MethodResult CanCreateNewspaper(Entity entity, string newspaperName, Citizen loggedCitizen);
        Newspaper CreateNewspaper(Entity owner, string newspaperName, int newspaperCountryID, Citizen loggedCitizen);
        MethodResult<Article> CreateArticle(Newspaper newspaper, Entity author, string title, string content, double? price, string payContent, string shortDescription, HttpPostedFileBase articleImage, bool publishStatus);
        MethodResult CreateComment(int articleID, int entityID, string content);
        void BuyArticle(Article article, Entity entity);
        NewspaperRightsEnum GetNewspaperRights(Newspaper newspaper, Entity entity, Citizen loggedCitizen);

        NewspaperJournalist AddNewJournalist(Newspaper newspaper, Citizen citizen);

        MethodResult CanAddJournalist(Newspaper newspaper, Citizen citizen);

        void ChangeRights(ChangeJournalistsRightsParam args);
        void RemoveJournalist(NewspaperJournalist journalist);
        void VoteOn(Article article, Entity entity, int score);
        int? GetVoteScore(int articleID, Entity entity);

        MethodResult CanChangeOwnership(Newspaper newspaper, Entity entity, Citizen loggedCitzen);
        MethodResult CanChangeOwnership(Newspaper newspaper, Entity entity, Citizen loggedCitizen, Entity newEntity);
        void ChangeOwnership(Newspaper newspaper, Entity newOwner);

        IEnumerable<EntityTypeEnum> GetEligibleEntityTypeForOwnership();
    }
}
