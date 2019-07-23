using System;
using Entities.enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Repository;
using Common.Operations;
using Entities.Extensions;
using SociatisCommon.Errors;
using System.Web.Mvc;
using WebServices.structs;
using WebServices.Classes.Constructions;

namespace WebServices
{
    public class ConstructionService : BaseService, IConstructionService
    {
        private readonly IDefenseSystemService defenseSystemService;
        private readonly IConstructionRepository constructionRepository;
        private readonly IHospitalService hospitalService;
        private readonly ICompanyService companyService;
        private readonly IReservedEntityNameRepository reservedEntityNameRepository;
        private readonly IRemovalService removalService;
        private readonly IWalletService walletService;
        private readonly IWarningService warningService;
        private readonly IWalletRepository walletRepository;

        public ConstructionService(IDefenseSystemService defenseSystemService, IConstructionRepository constructionRepository, ICompanyService companyService,
            IHospitalService hospitalService, IReservedEntityNameRepository reservedEntityNameRepository, IRemovalService removalService, IWalletService walletService, IWarningService warningService,
            IWalletRepository walletRepository)
        {
            this.defenseSystemService = defenseSystemService;
            this.constructionRepository = constructionRepository;
            this.companyService = companyService;
            this.hospitalService = hospitalService;
            this.reservedEntityNameRepository = reservedEntityNameRepository;
            this.removalService = removalService;
            this.walletService = walletService;
            this.warningService = warningService;
            this.walletRepository = walletRepository;
        }
        public int GetProgressNeededToBuild(Construction construction)
        {
            return GetProgressNeededToBuild(construction.Company.GetProducedProductType(), construction.Company.Quality);
        }
        public int GetProgressNeededToBuild(ProductTypeEnum constructionType, int quality)
        {
            double modifier = 0.01;
            switch (constructionType)
            {
                case ProductTypeEnum.DefenseSystem:
                    return (int)(defenseSystemService.GetNeededConstructionPoints(quality) * modifier);
                case ProductTypeEnum.Hotel:
                    return (int)(50_000 * modifier);
                case ProductTypeEnum.House:
                    return (int)(5_000 * modifier);
            }

            throw new NotImplementedException();
        }

        public MethodResult CanFinishOneTimeConstruction(Construction construction, Entity currentEntity, Citizen loggedCitizen)
        {
            if (construction == null)
                return new MethodResult("Construction does not exist!");

            var rights = companyService.GetCompanyRights(construction.Company, currentEntity, loggedCitizen);

            if (rights.CanAcceptConstruction == false)
                return new MethodResult("You cannot do that!");
            if (construction.Progress < GetProgressNeededToBuild(construction))
                return new MethodResult("Construction is not finished yet!");

            if (construction.Company.GetProducedProductType().IsOneTimeConstruction() == false)
                return new MethodResult("Those type of constructions are not to accept.");

            return MethodResult.Success;

        }
        public object FinishOneTimeConstruction(Construction construction)
        {
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                int quality = construction.Company.Quality;
                object _return = null;

                DissmissAllWorkersDueToBuildingFinish(construction);
                var onwerWallet = construction.Company.Owner.Wallet;
                List<Money> founds = walletRepository.GetMoney(construction.Company.Entity.WalletID)
                    .Select(m => new Money()
                    {
                        Currency = Persistent.Currencies.GetById(m.CurrencyID),
                        Amount = m.Amount
                    }).ToList();

                var finishWorker = ConstructionFinishWorkerProvider.Provide(construction);

                finishWorker.AcumulateDataBeforeCompanyDelete(construction);



                removalService.RemoveCompany(construction.Company);
                walletService.TransferAllFounds(founds, onwerWallet);

                finishWorker.ExecuteAction();
                walletRepository.SaveChanges();
                trs.Complete();
                return _return;
            }
        }

        public void DissmissAllWorkersDueToBuildingFinish(Construction construction)
        {
            foreach (var employee in construction.Company.CompanyEmployees.ToList())
            {
                var message = $"You are no longer building {construction.Company.Entity.Name} because construction has been finished!";
                warningService.AddWarning(employee.CitizenID, message);
                constructionRepository.RemoveSpecific(employee);
            }
        }

    }
}
