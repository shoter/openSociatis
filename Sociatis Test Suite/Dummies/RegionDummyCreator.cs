using Common.utilities;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class RegionDummyCreator : IDummyCreator<Region>
    {
        private Region region;
        private static UniqueIDGenerator idGenerator = new UniqueIDGenerator();

        public RegionDummyCreator()
        {
            region = createRegion();
        }

        public Region Create(Country country)
        {
            region.Country = country;
            region.CountryID = country.ID;
            region.CanSpawn = true;
            country.Regions.Add(region);

            var _return = region;
            region = createRegion();
            return _return;
        }

        public RegionDummyCreator AttachCountry(Country country)
        {
            region.CountryID = country.ID;
            country.Regions.Add(region);

            return this;
        }

        private Region createRegion()
        {
            return new Region()
            {
                ID = idGenerator.UniqueID,
                CanSpawn = true,
                DefenseSystemQuality = 5,
                Development = new decimal(5.0),
                Name = RandomGenerator.GenerateString(10)
            };
        }

        public Region Create()
        {
            var country = new CountryDummyCreator().Create();
            return Create(country);
        }
    }
}
