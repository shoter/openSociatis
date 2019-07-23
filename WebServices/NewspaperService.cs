using System;
using System.Linq;
using Common.Operations;
using Entities;
using Entities.Repository;
using Entities.Extensions;
using Entities.enums;
using Common;
using System.Web;
using WebServices.enums;
using WebServices.Helpers;
using WebServices.structs;
using System.Collections.Generic;
using Weber.Html;

namespace WebServices
{
    public class NewspaperService : BaseService, INewspaperService
    {
        private readonly INewspaperRepository newspaperRepository;
        private readonly IEntityService entityService;
        private readonly IEntityRepository entityRepository;
        private readonly IArticleRepository articleRepository;
        private readonly IUploadService uploadService;
        private readonly IWarningService warningService;
        private readonly ITransactionsService transactionService;
        private readonly IWalletService walletService;
        private readonly IConfigurationRepository configurationRepository;
        private readonly ICitizenService citizenService;

        public NewspaperService(INewspaperRepository newspaperRepository, IEntityService entityService, IEntityRepository entityRepository, IArticleRepository articleRepository,
            IUploadService uploadService, IWarningService warningService, ITransactionsService transactionService, IWalletService walletService, IConfigurationRepository configurationRepository,
            ICitizenService citizenService)
        {
            this.newspaperRepository = newspaperRepository;
            this.entityService = Attach(entityService);
            this.entityRepository = entityRepository;
            this.articleRepository = articleRepository;
            this.uploadService = Attach(uploadService);
            this.warningService = Attach(warningService);
            this.transactionService = Attach(transactionService);
            this.walletService = walletService;
            this.configurationRepository = configurationRepository;
            this.citizenService = Attach(citizenService);
        }

 
        public void RemoveJournalist(NewspaperJournalist journalist)
        {
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                string message = string.Format("You are no longer journalist in {0}.", journalist.Newspaper.Entity.Name);
                newspaperRepository.RemoveJournalist(journalist);
                warningService.AddWarning(journalist.CitizenID, message);

                newspaperRepository.SaveChanges();
                transaction.Complete();
            }
        }

        public void VoteOn(Article article, Entity entity, int score)
        {
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                var vote = articleRepository.GetVote(article.ID, entity.EntityID);
                if (vote != null)
                {
                    article.VoteScore -= vote.Score;
                    articleRepository.Remove(vote);
                }

                articleRepository.AddVote(article.ID, entity.EntityID, score);
                article.VoteScore += score;
                articleRepository.SaveChanges();
                transaction.Complete();
            }

        }
        public int? GetVoteScore(int articleID, Entity entity)
        {
            var vote = articleRepository.GetVote(articleID, entity.EntityID);
            return vote?.Score;
        }
        public MethodResult<Article> CreateArticle(Newspaper newspaper, Entity author, string title, string content, double? price, string payContent, string shortDescription, HttpPostedFileBase articleImage, bool publishStatus)
        {
            var result = MethodResult<Article>.Success;

            string imgUrl = "";
            if(articleImage != null && string.IsNullOrWhiteSpace(articleImage.FileName) == false)
            {
                MethodResult<string> file = uploadService.UploadImage(articleImage, UploadLocationEnum.Articles);
                if (file.IsError)
                    return result.Merge(file);
                imgUrl = file;
            }

            Article article = new Article()
            {
                Newspaper = newspaper,
                CreationDate = DateTime.Now,
                AuthorID = author.EntityID,
                VoteScore = 0,
                Title = title,
                Content = content,
                ImgURL = imgUrl,
                ShortDescription = shortDescription,
                CreationDay = GameHelper.CurrentDay,
                PayOnlyContent = payContent ?? "",
                Price = (decimal?)price,
                Published = publishStatus

            };

            articleRepository.Add(article);
            articleRepository.SaveChanges();

            result.ReturnValue = article;
            return result;
        }

        public void BuyArticle(Article article, Entity entity)
        {
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                if (article.Price.HasValue)
                {
                    var currency = Persistent.Countries.GetCountryCurrency(article.Newspaper.Country);
                    var taxPercentage = (double)article.Newspaper.Country.CountryPolicy.ArticleTax;
                    var price = (double)article.Price.Value;
                    var priceWithTax = Math.Round(price * (1 + taxPercentage), 2);
                    var tax = priceWithTax - price;
                    transactionService.PayForArticle(article, entity, currency.ID, price, tax);
                    warningService.AddWarning(article.NewspaperID, string.Format("{0} bought article \"{1}\" for {2} {3}", entity.Name, article.Title, article.Price, currency.Symbol));
                    article.GeneratedIncome += article.Price.Value;
                }

                if (article.Newspaper.Owner.EntityTypeID == (int)EntityTypeEnum.Citizen)
                    citizenService.OnSoldArticle(article.Newspaper.Owner.Citizen);

                articleRepository.AddBuyer(article.ID, entity.EntityID);
                articleRepository.SaveChanges();
                transaction.Complete();
            }
        }

        public NewspaperRightsEnum GetNewspaperRights(Newspaper newspaper, Entity entity, Citizen loggedCitizen)
        {
            if (entity.EntityID == newspaper.OwnerID || entity.EntityID == newspaper.ID)
                return NewspaperRightsEnum.Full;
            if (entity.GetEntityType() != EntityTypeEnum.Citizen)
                return NewspaperRightsEnum.None;

            var journalist = newspaper.NewspaperJournalists.FirstOrDefault(j => j.CitizenID == loggedCitizen.ID);

            if (journalist == null)
                return NewspaperRightsEnum.None;

            return journalist.GetRights();
        }

        public MethodResult CanChangeOwnership(Newspaper newspaper, Entity entity, Citizen loggedCitzen)
        {
            if (newspaper == null)
                return new MethodResult("Newspaper does not exist!");

            var rights = GetNewspaperRights(newspaper, entity, loggedCitzen);

            if (rights == NewspaperRightsEnum.Full)
                return MethodResult.Success;

            return new MethodResult("You do not have sufficient rights to do this");
        }

        public IEnumerable<EntityTypeEnum> GetEligibleEntityTypeForOwnership()
        {
            yield return EntityTypeEnum.Citizen;
            yield return EntityTypeEnum.Organisation;
            yield return EntityTypeEnum.Company;
        }

        public MethodResult CanChangeOwnership(Newspaper newspaper, Entity entity, Citizen loggedCitizen, Entity newEntity)
        {
            if (newEntity == null)
                return new MethodResult("New owner does not exist!");
            if (newspaper == null)
                return new MethodResult("Newspaper does not exist!");
            if (newEntity.EntityID == newspaper.OwnerID)
                return new MethodResult($"{newEntity.Name} is already an owner!");

            if (GetEligibleEntityTypeForOwnership().Contains(newEntity.GetEntityType()) == false)
                return new MethodResult($"{newEntity.Name} cannot be an ownership of this newspaper!");

            return CanChangeOwnership(newspaper, entity, loggedCitizen);
        }

        public void ChangeOwnership(Newspaper newspaper, Entity newOwner)
        {
            newspaper.OwnerID = newOwner.EntityID;
            var newspaperLink = EntityLinkCreator.Create(newspaper.Entity);
            string message = $"You are new owner of {newspaperLink}.";
            using (NoSaveChanges)
                warningService.AddWarning(newOwner.EntityID, message);

            ConditionalSaveChanges(newspaperRepository);
        }
        
        public MethodResult CreateComment(int articleID, int entityID, string content)
        {
            ArticleComment comment = new ArticleComment()
            {
                ArticleID = articleID,
                AuthorID = entityID,
                Content = content,
                CreationDate = DateTime.Now,
                CreationDay = GameHelper.CurrentDay
            };

            articleRepository.AddComment(comment);
            articleRepository.SaveChanges();
            return MethodResult.Success;
        }

        public MethodResult CanCreateNewspaper(Entity entity)
        {
            var result = MethodResult.Success;

            switch(entity.GetEntityType())
            {
                //It's better to have list of who is not allowed to create newspaper insteaed of who is not allowed ,
                //because when i do not allowed list then every new entity type will be allowed to make newspaper as it want to.
                case EntityTypeEnum.Citizen:
                case EntityTypeEnum.Company:
                case EntityTypeEnum.Country:
                case EntityTypeEnum.Organisation:
                case EntityTypeEnum.Party:
                    break;
                default:
                    result.AddError("You cannot create newspaper as {0}", entity.GetEntityType().ToHumanReadable());
                    break;
            }

            return result;
        }

        public Newspaper CreateNewspaper(Entity owner, string newspaperName, int newspaperCountryID, Citizen loggedCitizen)
        {
            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                Entity entity = entityService.CreateEntity(newspaperName, EntityTypeEnum.Newspaper);
                entity.Equipment.ItemCapacity = 1000;
                var newspaper = new Newspaper()
                {
                    CountryID = newspaperCountryID,
                    OwnerID = owner.EntityID,
                    Entity = entity
                };

                newspaperRepository.Add(newspaper);
                newspaperRepository.SaveChanges();

                if (owner.GetEntityType() != EntityTypeEnum.Party)
                {
                    transactionService.PayForNewspaperCreation(newspaper, owner);
                }
                else
                {
                    transactionService.PayForNewspaperCreation(newspaper, loggedCitizen.Entity);
                }

                transaction.Complete();

                return newspaper;
            }
        }

        public MethodResult CanCreateNewspaper(Entity entity, string newspaperName, Citizen loggedCitizen)
        {
            var country = entity.GetCurrentCountry();
            var newspaperCreationFee = configurationRepository.First().NewspaperCreationFee;
            var other = entityRepository.FirstOrDefault(e => e.Name.ToLower() == newspaperName.ToLower());

            if (entityService.IsNameTaken(newspaperName))
                return new MethodResult("Name is already used!");

            if (entityService.IsSpecialName(newspaperName))
            {
                return new MethodResult("This is special name. You cannot use it");
            }

            var money = new Money()
            {
                Amount = country.CountryPolicy.NewspaperCreateCost,
                Currency = Persistent.Countries.GetCountryCurrency(country)
            };

            var adminMoney = new Money()
            {
                Amount = newspaperCreationFee,
                Currency = Persistent.Currencies.GetById((int)CurrencyTypeEnum.Gold)
            };
            if (entity.GetEntityType() != EntityTypeEnum.Party)
            {
                if (walletService.HaveMoney(entity.WalletID, money, adminMoney) == false)
                    return new MethodResult("You do not have enough money to create newspaper!");
            }
            else if (walletService.HaveMoney(loggedCitizen.Entity.WalletID, money, adminMoney) == false)
                return new MethodResult("You do not have enough money to create newspaper!");

            return MethodResult.Success;
        }

        public NewspaperJournalist AddNewJournalist(Newspaper newspaper, Citizen citizen)
        {
            NewspaperJournalist journalist = new NewspaperJournalist()
            {
                CanWriteArticles = false,
                CanManageArticles = false,
                CanManageJournalists = false,
                Citizen = citizen,
                Newspaper = newspaper
            };

            newspaperRepository.AddJournalist(journalist);
            newspaperRepository.SaveChanges();
            return journalist;
        }

        public MethodResult CanAddJournalist(Newspaper newspaper, Citizen citizen)
        {
            var result = MethodResult.Success;
            if (newspaper.OwnerID == citizen.ID)
                result.AddError("Newspaper owner cannot be a journalist");
            else if (newspaper.NewspaperJournalists.Any(j => j.CitizenID == citizen.ID))
                result.AddError("He is already employed as journalist");
            return result;
        }

        public void ChangeRights(ChangeJournalistsRightsParam args)
        {
            var journalistIDs = new List<int>();
            foreach (var right in args.Rights)
                journalistIDs.Add(right.Key);

            var oldJournalists = newspaperRepository.GetJournalists(journalistIDs, args.Newspaper.ID);

            using (var transaction = transactionScopeProvider.CreateTransactionScope())
            {
                using (NoSaveChanges)
                {
                    foreach (var right in args.Rights)
                    {
                        var journalist = oldJournalists.First(j => j.CitizenID == right.Key);
                        var message = generateChangeRightsMessage(args.Newspaper, journalist.GetRights(), right.Value);
                        if (message == null)
                            continue;

                        newspaperRepository.ChangeJournalistRights(journalist.CitizenID, args.Newspaper.ID, right.Value);
                        warningService.AddWarning(journalist.CitizenID, message);
                    }
                }

                newspaperRepository.SaveChanges();
                transaction.Complete();
            }

        }

        private static string generateChangeRightsMessage(Newspaper newspaper,  NewspaperRightsEnum oldRights, NewspaperRightsEnum newRights)
        {
            if (hasChanged(oldRights, newRights) == false)
                return null;

            string message = string.Format("Your rights in {0} were changed. Your current rights: {1}", newspaper.Entity.Name, newRights.ToHumanReadable());

            if (message.Last() == ',')
                message = message.Substring(0, message.Length - 1);

            return message + ".";
        }


        /// <summary>
        /// Does not support FULL enum
        /// </summary>
        private static bool hasChanged(NewspaperRightsEnum oldRights, NewspaperRightsEnum newRights)
        {
            return oldRights != newRights;
        }
    }
}
