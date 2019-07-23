using Common.Operations;
using Common.Transactions;
using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;
using WebServices.structs;
using Common.Extensions;

namespace WebServices
{
    public class DefenseSystemService : BaseService, IDefenseSystemService
    {
        private readonly IWalletService walletService;
        private readonly IConstructionRepository constructionRepository;
        private readonly ICompanyService companyService;
        public DefenseSystemService(IWalletService walletService, IConstructionRepository  constructionRepository, ITransactionScopeProvider transactionScopeProvider,
            ICompanyService companyService)
        {
            this.walletService = Attach(walletService);
            this.constructionRepository = constructionRepository;
            this.companyService = companyService;
        }
        public decimal GetGoldCostForStartingConstruction(int quality)
        {
            switch (quality)
            {
                case 1:
                    return 2.5m;
                case 2:
                    return 5;
                case 3:
                    return 10;
                case 4:
                    return 25;
                case 5:
                    return 75;
            }
            throw new ArgumentException("quality");
        }

        public int GetNeededConstructionPoints(int quality)
        {
            int modifier = 10;
            switch (quality)
            {
                case 1:
                    return 500 / modifier;
                case 2:
                    return 2500 / modifier;
                case 3:
                    return 12_500 / modifier;
                case 4:
                    return 50_000 / modifier;
                case 5:
                    return 200_000 / modifier;
            }
            throw new ArgumentException("quality");
        }

        public MethodResult CanBuildDefenseSystem(Region region, Country country, int quality)
        {
            if (region == null)
                return new MethodResult("Region does not exist!");
            if (country == null)
                return new MethodResult("Country does not exist!");
            if (region.CountryID != country.ID)
                return new MethodResult("Region does not belongs to your country!");

            if (region.DefenseSystemQuality >= quality)
                return new MethodResult("You cannot construct defense system of this quality here!");

            var money = new Money(GameHelper.Gold, GetGoldCostForStartingConstruction(quality));
            if (walletService.HaveMoney(country.Entity.WalletID, money) == false)
                return new MethodResult($"Your country does not have {money.Amount} gold!");

            if (constructionRepository.AnyConstructionTypeBuildInRegion(region.ID, ProductTypeEnum.DefenseSystem))
                return new MethodResult("Defense system is already under construction in this region!");

            return MethodResult.Success;


        }

        public void BuildDefenseSystem(Region region, Country country, int quality, IConstructionService constructionService)
        {
            var name = Constants.DefenseSystemConstructionName.FormatString(region.Name);
            using (var trs = transactionScopeProvider.CreateTransactionScope())
            {
                var company = companyService.CreateCompany(name, ProductTypeEnum.DefenseSystem, region.ID, country.ID);
                company.Quality = quality;
              
                constructionRepository.SaveChanges();
                trs.Complete();
            }

        }
    }
}
