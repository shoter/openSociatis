using Common.utilities;
using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class CitizenDummyCreator : EntityDummyCreator, IDummyCreator<Citizen>
    {
        private readonly CountryDummyCreator countryCreator = new CountryDummyCreator();
        private Citizen citizen
        {
            get { return entity.Citizen; }
            set
            {
                value.Entity = entity;
                value.ID = entity.EntityID;
                entity.Citizen = value;
            }
        }


        public CitizenDummyCreator()
        {
            createCitizen();
        }

        public new Citizen Create()
        {
            var _return = citizen;
            createCitizen();
            return _return;
        }

        new public CitizenDummyCreator SetName(string name)
        {
            base.SetName(name);
            return this;
        }

        

        private void createCitizen()
        {
            base.createEntity(EntityTypeEnum.Citizen);
            citizen = new Citizen();
            SetCountry(countryCreator.Create());
        }

        public CitizenDummyCreator SetCountry(Country country)
        {
            SetRegion(country.Regions.First());
            citizen.CitizenshipID = country.ID;
            citizen.Country = country;

            return this;
        }

        public CitizenDummyCreator SetRegion(Region region)
        {
            citizen.Region = region;
            citizen.RegionID = region.ID;
            region.Citizens.Add(citizen);

            return this;
        }
    }
}
