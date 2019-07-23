using Entities;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Country
{
    public class CountryMPPListViewModel
    {
        public CountryInfoViewModel Info { get; set; }

        public List<MPPViewModel> MPPs { get; set; } = new List<MPPViewModel>();

        public List<int> RuledCountries { get; set; } = new List<int>();
        public bool CanCreateMPPOffers { get; set; } 


        public CountryMPPListViewModel(Entities.Country country, List<MilitaryProtectionPact> pacts,
            List<MilitaryProtectionPactOffer> proposedList, List<MilitaryProtectionPactOffer> proposals)
        {
            Info = new CountryInfoViewModel(country);
            foreach (var pact in pacts)
                MPPs.Add(new MPPViewModel(pact));
            foreach (var proposed in proposedList)
                MPPs.Add(new MPPViewModel(proposed, isProposal: false));
            foreach (var proposal in proposals)
                MPPs.Add(new MPPViewModel(proposal, true));

            var mppService = DependencyResolver.Current.GetService<IMPPService>();

            RuledCountries = mppService.GetListOfCountriesWhereCitizenCanManageMPPs(SessionHelper.LoggedCitizen)
                .Select(c => c.ID).ToList();

            CanCreateMPPOffers = mppService.CanOfferMPP(SessionHelper.CurrentEntity, country).isSuccess;
        }
    }
}