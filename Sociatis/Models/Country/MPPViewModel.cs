using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices;

namespace Sociatis.Models.Country
{
    public class MPPViewModel
    {
        public int ID { get; set; }

        public bool IsProposal { get; set; }
        public bool IsProposed { get; set; }

        public string FirstCountryName { get; set; }
        public int FirstCountryID { get; set; }
        public string SecondCountryName { get; set; }
        public int SecondCountryID { get; set; }

        public int StartDay { get; set; }


        public string StatusTxt { get; set; }

        public MPPViewModel(MilitaryProtectionPact mpp)
        {
            IsProposal = IsProposed = false;
            var firstCountry = Persistent.Countries.GetById(mpp.FirstCountryID);
            var secondCountry = Persistent.Countries.GetById(mpp.SecondCountryID);

            FirstCountryName = firstCountry.Entity.Name;
            FirstCountryID = firstCountry.ID;

            SecondCountryName = secondCountry.Entity.Name;
            SecondCountryID = secondCountry.ID;

            StatusTxt = $"Will end at day {mpp.EndDay}";

            ID = mpp.ID;
            StartDay = mpp.StartDay;
        }

        public MPPViewModel(MilitaryProtectionPactOffer offer, bool isProposal)
        {
            IsProposal = isProposal;
            IsProposed = !isProposal;
            var firstCountry = Persistent.Countries.GetById(offer.FirstCountryID);
            var secondCountry = Persistent.Countries.GetById(offer.SecondCountryID);

            FirstCountryName = firstCountry.Entity.Name;
            FirstCountryID = firstCountry.ID;

            SecondCountryName = secondCountry.Entity.Name;
            SecondCountryID = secondCountry.ID;

            ID = offer.ID;
            StartDay = offer.Day;
        }
    }
}