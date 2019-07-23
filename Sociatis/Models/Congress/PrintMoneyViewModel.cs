using Common.Exceptions;
using Entities;
using Entities.enums;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebServices.structs;
using WebServices.structs.Votings;

namespace Sociatis.Models.Congress
{
    public class PrintMoneyViewModel : CongressVotingViewModel
    {
        [DisplayName("Money amount to print")]
        [Range(1, 100000)]
        public int MoneyAmount { get; set; }
        public double CostOfThousand { get; set; }
        public string CurrencySymbol { get; set; }


        public override VotingTypeEnum VotingType { get; set; } = VotingTypeEnum.PrintMoney;

        public PrintMoneyViewModel() { }
        public PrintMoneyViewModel(int countryID) : base(countryID)
        {
            init();
        }
        public PrintMoneyViewModel(CongressVoting voting)
        : base(voting)
        {
            init();
        }

        public PrintMoneyViewModel(FormCollection values)
        : base(values)
        {
            init();
        }

        void init()
        {

            var congressVotingService = DependencyResolver.Current.GetService<ICongressVotingService>();
            CostOfThousand = congressVotingService.GetPrintCost(1000);

            CurrencySymbol = Persistent.Countries.GetCountryCurrency(CountryID).Symbol;
        }



        public override StartCongressVotingParameters CreateVotingParameters()
        {
            var congressVotingService = DependencyResolver.Current.GetService<ICongressVotingService>();
            var walletService = DependencyResolver.Current.GetService<IWalletService>();
            var entityRepository = DependencyResolver.Current.GetService<IEntityRepository>();

            double goldCost = congressVotingService.GetPrintCost(MoneyAmount);
            var walletID = entityRepository.Where(e => e.EntityID == CountryID)
                .Select(e => e.WalletID).First();

            if (walletService.HaveMoney(walletID, new Money(GameHelper.Gold, (decimal)goldCost)) == false)
                throw new UserReadableException($"Country does not have enough gold! It needs {goldCost} gold.");


            return new PrintMoneyVotingParameters(MoneyAmount);
        }
    }
}