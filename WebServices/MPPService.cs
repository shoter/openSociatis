using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations;
using Entities;
using WebServices.structs;
using WebServices.Helpers;
using Entities.Repository;
using Weber.Html;

namespace WebServices
{
    public class MPPService : BaseService, IMPPService
    {
        private readonly ICountryService countryService;
        private readonly IWalletService walletService;
        private readonly IWarService warService;
        private readonly IMilitaryProtectionPactOfferRepository militaryProtectionPactOfferRepository;
        private readonly IMilitaryProtectionPactRepository militaryProtectionPactRepository;
        private readonly ITransactionsService transactionsService;
        private readonly IWarningService warningService;

        public MPPService(ICountryService countryService, IWalletService walletService, IWarService warService,
            IMilitaryProtectionPactOfferRepository militaryProtectionPactOfferRepository, ITransactionsService transactionsService,
            IWarningService warningService, IMilitaryProtectionPactRepository militaryProtectionPactRepository)
        {
            this.countryService = countryService;
            this.walletService = walletService;
            this.warService = warService;
            this.militaryProtectionPactOfferRepository = militaryProtectionPactOfferRepository;
            this.transactionsService = transactionsService;
            this.warningService = warningService;
            this.militaryProtectionPactRepository = militaryProtectionPactRepository;
        }

        public decimal CalculateMPPCost(int days)
        {
            decimal cost = 1m;
            for (int i = 0; i < days; ++i)
            {
                cost += Math.Max(1m - i * 0.020m, 0.01m);
            }
            return cost;
        }


        public MethodResult CanOfferMPP(Entity entity, Country proposingCountry)
        {
            if (proposingCountry == null)
                return new MethodResult("Country does not exist!");
            
            if (countryService.IsPresident(proposingCountry, entity) == false)
                return new MethodResult("You are not a president!");


            return MethodResult.Success;
        }


        public MethodResult CanOfferMPP(Entity entity, Country proposingCountry, Country secondCountry, int days)
        {
            if (secondCountry == null)
                return new MethodResult("Country does not exist!");
            var result = CanOfferMPP(entity, proposingCountry);
            if (result.IsError)
                return result;

            if (proposingCountry.ID == secondCountry.ID)
                return new MethodResult("You cannot have MPP with yourself!");

            if (militaryProtectionPactRepository.AreAlreadyHaveMPP(proposingCountry.ID, secondCountry.ID))
                return new MethodResult("You already have MPP!");

            if (militaryProtectionPactOfferRepository.AreAlreadyHaveMPP(proposingCountry.ID, secondCountry.ID))
                return new MethodResult("You already have MPP offer between you!");



            var minLength = ConfigurationHelper.Configuration.MinimumMPPLength;
            var maxLength = ConfigurationHelper.Configuration.MaximumMPPLength;

            if (days < minLength)
                return new MethodResult($"Days must be bigger than {minLength}!");
            else if(days > maxLength)
                return new MethodResult($"Days must be lower than {maxLength}!");

            var goldCost = CalculateMPPCost(days);
            var money = new Money(GameHelper.Gold, goldCost);

            if (walletService.HaveMoney(proposingCountry.Entity.WalletID, money) == false)
                return new MethodResult($"{proposingCountry.Entity.Name} does not have enough gold! It needs {goldCost} gold.");

            if (warService.AreAtWar(proposingCountry, secondCountry))
                return new MethodResult("Both countries are at war. You cannot do that.");

            return MethodResult.Success;
        }

        public void OfferMPP(Entity entity, Country proposingCountry, Country secondCountry, int days)
        {
            var goldCost = CalculateMPPCost(days);

            transactionsService.PayForMPPOffer(proposingCountry, secondCountry, goldCost);

            var offer = new MilitaryProtectionPactOffer()
            {
                Day = GameHelper.CurrentDay,
                FirstCountryID = proposingCountry.ID,
                SecondCountryID = secondCountry.ID,
                Length = days,
                ReservedGold = goldCost
            };

            militaryProtectionPactOfferRepository.Add(offer);
            militaryProtectionPactOfferRepository.SaveChanges();
        }

        public IEnumerable<Country> GetListOfCountriesWhereCitizenCanManageMPPs(Citizen citizen)
        {
            var countriesIDs = citizen.CountriesPresident.Select(cp => cp.ID).ToList();

            foreach (var countryID in countriesIDs)
            {
                yield return Persistent.Countries.GetById(countryID);
            }
        }


        public MethodResult CanAcceptMPP(MilitaryProtectionPactOffer offer, Entity entity)
        {
            return haveRightsToMPPOffer(offer, entity);
        }

        private MethodResult haveRightsToMPPOffer(MilitaryProtectionPactOffer offer, Entity entity)
        {
            if (offer == null)
                return new MethodResult("Offer does not exist!");

            if (countryService.IsPresident(offer.SecondCountry, entity) == false)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public MethodResult CanRefuseMPP(MilitaryProtectionPactOffer offer, Entity entity)
        {
            return haveRightsToMPPOffer(offer, entity);
        }

        public void RefuseMPP(MilitaryProtectionPactOffer offer)
        {
            transactionsService.GetGoldForDeclineMPP(offer.FirstCountry, offer.SecondCountry, offer.ReservedGold);

            var otherLink = EntityLinkCreator.Create(offer.SecondCountry.Entity);
            var msg = $"{otherLink} declined your MPP offer.";
            warningService.AddWarning(offer.FirstCountryID, msg);


            militaryProtectionPactOfferRepository.Remove(offer);
            militaryProtectionPactOfferRepository.SaveChanges();
        }

        public MethodResult CanCancelMPP(MilitaryProtectionPactOffer offer, Entity entity)
        {
            if (offer == null)
                return new MethodResult("Offer does not exist!");

            if (countryService.IsPresident(offer.FirstCountry, entity) == false)
                return new MethodResult("You cannot do that!");

            return MethodResult.Success;
        }

        public void CancelMPP(MilitaryProtectionPactOffer offer)
        {
            var otherLink = EntityLinkCreator.Create(offer.FirstCountry.Entity);
            var msg = $"{otherLink} has canceled military pact proposal";
            warningService.AddWarning(offer.SecondCountryID, msg);

            transactionsService.GetGoldForDeclineMPP(offer.FirstCountry, offer.SecondCountry, offer.ReservedGold);

            militaryProtectionPactOfferRepository.Remove(offer);
            militaryProtectionPactOfferRepository.SaveChanges();
        }

        public void AcceptMPP(MilitaryProtectionPactOffer offer)
        {
            var otherLink = EntityLinkCreator.Create(offer.SecondCountry.Entity);
            var msg = $"{otherLink} accepted your MPP offer.";
            warningService.AddWarning(offer.FirstCountryID, msg);

            var pact = new MilitaryProtectionPact()
            {
                Active = true,
                StartDay = GameHelper.CurrentDay,
                EndDay = GameHelper.CurrentDay + offer.Length,
                FirstCountryID = offer.FirstCountryID,
                SecondCountryID = offer.SecondCountryID
            };

            militaryProtectionPactRepository.Add(pact);
            militaryProtectionPactOfferRepository.Remove(offer);
            militaryProtectionPactOfferRepository.SaveChanges();
        }

        public virtual void EndMPP(MilitaryProtectionPact mpp)
        {
            var firstCountry = Persistent.Countries.GetById(mpp.FirstCountryID);
            var secondCountry = Persistent.Countries.GetById(mpp.SecondCountryID);

            var firstLink = EntityLinkCreator.Create(firstCountry.Entity);
            var secondLink = EntityLinkCreator.Create(secondCountry.Entity);

            var msg = $"MPP between {firstLink} and {secondLink} has ended.";

            warningService.AddWarning(firstCountry.ID, msg);
            warningService.AddWarning(secondCountry.ID, msg);

            militaryProtectionPactRepository.Remove(mpp);
            ConditionalSaveChanges(militaryProtectionPactRepository);
        }

        public void ProcessDayChange(int newDay)
        {
            var activeMPPs = militaryProtectionPactRepository.Where(mpp => mpp.Active && mpp.EndDay <= newDay).ToList();

            using (NoSaveChanges)
                foreach (var mpp in activeMPPs)
                    EndMPP(mpp);

            militaryProtectionPactRepository.SaveChanges();
        }


        


    }
}
