using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class EmbargoDummyCreator : IDummyCreator<Embargo>
    {
        private static UniqueIDGenerator IDGen = new UniqueIDGenerator();
        CountryDummyCreator countryCreator = new CountryDummyCreator();

        Embargo dummy;

        public EmbargoDummyCreator()
        {
            dummy = makeDefault();
        }




        public Embargo Create()
        {
            var temp = dummy;
            dummy = makeDefault();
            return temp;

        }

        private Embargo makeDefault()
        {
            var embargo = new Embargo()
            {
                Active = true,
                StartDay = 0,
                StartTime = DateTime.Now,
                ID = IDGen.UniqueID
            };

            var creatorCountry = countryCreator.Create();
            var embargoedCountry = countryCreator.Create();

            embargo.CreatorCountry = creatorCountry;
            creatorCountry.CreatedEmbargoes.Add(embargo);
            embargo.CreatorCountryID = creatorCountry.ID;

            embargo.EmbargoedCountry = embargoedCountry;
            embargoedCountry.Embargoes.Add(embargo);
            embargo.EmbargoedCountryID = embargoedCountry.ID;

            return embargo;
        }

    }
}
