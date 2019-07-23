using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using Entities.enums;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewTransferCashToCompanyViewModel : ViewVotingBaseViewModel
    {
        public string CurrencySymbol { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public decimal Amount { get; set; }


        public ViewTransferCashToCompanyViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        : base(voting, isPlayerCongressman, canVote)
        {
            Amount = decimal.Parse(voting.Argument1);
            CompanyID = int.Parse(voting.Argument2);
            var currencyID = int.Parse(voting.Argument3);

            CurrencySymbol = Persistent.Currencies.GetById(currencyID).Symbol;

            var companyRepository = DependencyResolver.Current.GetService<ICompanyRepository>();
            CompanyName = companyRepository.Where(c => c.ID == CompanyID)
                .Select(c => c.Entity.Name).First();
        }
    }
}