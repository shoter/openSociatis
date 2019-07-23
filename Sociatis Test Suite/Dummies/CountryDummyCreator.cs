using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis_Test_Suite.Dummies
{
    public class CountryDummyCreator : EntityDummyCreator, IDummyCreator<Country>
    {
        CurrencyDummyCreator currencyGenerator = new CurrencyDummyCreator();
        RegionDummyCreator regionCreator = new RegionDummyCreator();

        /// <summary>
        /// Do not access create, please?
        /// </summary>
        public PresidentVotingDummyCreator VotingCreator { get; set; } = new PresidentVotingDummyCreator();

        private Country country
        {
            get { return entity.Country; }
            set
            {
                value.Entity = entity;
                value.ID = entity.EntityID;
                entity.Country = value;
            }
        }

        public CountryDummyCreator()
        {
            createCountry();
        }

        public new Country Create()
        {
            country.PresidentVotings.Add(VotingCreator.Create(country));

            var _return = country;
            createCountry();

            Persistent.Countries.Add(_return);
            return _return;
        }

        public CountryDummyCreator CreateNewRegion()
        {
            regionCreator.Create(country);

            return this;
        }


        private void createCountry()
        {
            base.createEntity(EntityTypeEnum.Country);
            country = new Country();
            country.CountryPolicy = new CountryPolicy()
            {
                PresidentCadenceLength = 30
            };

            country.Currency = currencyGenerator.Create();
            CreateNewRegion();

            country.Capital = country.Regions.First();
            country.CapitalID = country.Capital.ID;
        }

        public CountryDummyCreator SetPresidentVotingStatus(int votingDay, VotingStatusEnum votingStatus)
        {
            VotingCreator.SetState(votingDay, votingStatus);

             return this;
        }
    }
}
