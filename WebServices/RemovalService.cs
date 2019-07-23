using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Repository;
using Weber.Html;
using Common;
using System.Transactions;

namespace WebServices
{
    public class RemovalService : BaseService, IRemovalService
    {
        private readonly IEntityRepository entityRepository;
        private readonly ITradeService tradeService;
        private readonly ICompanyEmployeeRepository companyEmployeeRepository;
        private readonly IWarningService warningService;


        public RemovalService(IEntityRepository entityRepository, ITradeService tradeService, ICompanyEmployeeRepository companyEmployeeRepository, IWarningService warningService)
        {
            this.entityRepository = entityRepository;
            this.tradeService = tradeService;
            this.companyEmployeeRepository = companyEmployeeRepository;
            this.warningService = warningService;
        }

        public void RemoveCompany(Company company)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var employees = company.CompanyEmployees.ToList();
                var companyLink = EntityLinkCreator.Create(company.Entity);
                foreach (var employee in employees)
                {
                    string message = $"Company {companyLink} was removed. You were fired due to this reason.";
                    warningService.AddWarning(employee.CitizenID, message);
                    entityRepository.RemoveSpecific(employee);

                }
                foreach (var manager in company.CompanyManagers.ToList())
                {
                    string message = $"Company {companyLink} was removed. You are not longer a manager in this company.";
                    warningService.AddWarning(manager.EntityID, message);
                    entityRepository.RemoveSpecific(manager);
                }

                foreach (var offer in company.MarketOffers.ToList())
                    entityRepository.RemoveSpecific(offer);
                foreach (var offer in company.JobOffers.ToList())
                {
                    if (offer.ContractJobOffer != null)
                        entityRepository.RemoveSpecific(offer.ContractJobOffer);
                    if (offer.NormalJobOffer != null)
                        entityRepository.RemoveSpecific(offer.NormalJobOffer);
                    entityRepository.RemoveSpecific(offer);
                }

                if (company.Hospital != null)
                    entityRepository.RemoveSpecific(company.Hospital);


                var entity = company.Entity;
                using (NoSaveChanges)
                    entityRepository.RemoveSpecific(company);
                RemoveEntity(entity);

                entityRepository.SaveChanges();
                trs.Complete();
            }
        }

        public void RemoveEntity(Entity entity)
        {
            foreach (var warning in entity.Warnings.ToList())
                entityRepository.RemoveSpecific(warning);

            foreach (var message in entity.MailboxMessages.ToList())
                entityRepository.RemoveSpecific(message);

            foreach (var sourceTransaction in entity.SourceLogs.ToList())
                sourceTransaction.SourceEntityID = null;
            foreach (var destinationTransaction in entity.DestinationLogs.ToList())
                destinationTransaction.DestinationEntityID = null;

            RemoveGiftTransactions(entity);


            foreach (var trade in entity.SourceTrades.ToList())
            {
                CloseTrade(trade, entity.EntityID);
            }

            foreach (var trade in entity.DestinationTrades.ToList())
            {
                CloseTrade(trade, entity.EntityID);
            }

            foreach (var comp in entity.OwnedCompanies.ToList())
                comp.OwnerID = null;

            foreach (var trans in entity.SellerMonetaryTransactions.ToList())
                trans.SellerID = null;
            foreach (var trans in entity.BuyerMonetaryTransactions.ToList())
                trans.BuyerID = null;

            var wallet = entity.Wallet;
            removeWallet(wallet);
            entityRepository.RemoveSpecific(entity);
            ConditionalSaveChanges(entityRepository);
        }
        private void removeWallet(Wallet wallet)
        {
            foreach (var trans in wallet.SourceTransactionLogs.ToList())
                trans.SourceWalletID = null;
            foreach (var trans in wallet.DestinationTransactionLogs.ToList())
                trans.DestinationWalletID = null;

            entityRepository.RemoveSpecific(wallet);
        }

        private void RemoveGiftTransactions(Entity entity)
        {
            foreach (var transactionLogs in entity.GiftTransactions.ToList())
                entityRepository.RemoveSpecific(transactionLogs);
            foreach (var transactionLogs in entity.GiftTransactions1.ToList())
                entityRepository.RemoveSpecific(transactionLogs);
        }

        public void CloseTrade(Trade trade, int removedEntityID)
        {
            tradeService.AbortTrade(trade, reason: "removal of company");
            if (trade.DestinationEntityID == removedEntityID)
                trade.DestinationEntityID = null;
            else
                trade.SourceEntityID = null;
        }

    }
}
