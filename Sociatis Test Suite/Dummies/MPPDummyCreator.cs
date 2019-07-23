using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Helpers;

namespace Sociatis_Test_Suite.Dummies
{
    public class MPPDummyCreator : IDummyCreator<MilitaryProtectionPact>
    {
        private MilitaryProtectionPact mpp;
        private static readonly UniqueIDGenerator uniqueID = new UniqueIDGenerator();
        private CountryDummyCreator countryCreator = new CountryDummyCreator();

        public MPPDummyCreator()
        {
            mpp = create();
        }


        private MilitaryProtectionPact create()
        {
            var firstCountry = countryCreator.Create();
            var secondCountry = countryCreator.Create();

            var mpp = new MilitaryProtectionPact()
            {
                FirstCountry = firstCountry,
                FirstCountryID = firstCountry.ID,
                StartDay = GameHelper.CurrentDay,
                EndDay = GameHelper.CurrentDay + 10,
                Active = true,
                ID = uniqueID,
                SecondCountry = secondCountry,
                SecondCountryID = secondCountry.ID
            };

            firstCountry.MPPsFirstCountry.Add(mpp);
            secondCountry.MPPsSecondCountry.Add(mpp);

            return mpp;
        }

        public MPPDummyCreator SetStartDay(int startDay)
        {
            mpp.StartDay = startDay;
            return this;
        }

        public MPPDummyCreator SetEndDay(int endDay)
        {
            mpp.EndDay = endDay;
            return this;
        }
        public MilitaryProtectionPact Create()
        {
            var temp = mpp;
            mpp = create();
            return temp;
        }
    }
}
