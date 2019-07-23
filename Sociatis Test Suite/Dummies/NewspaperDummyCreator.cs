using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class NewspaperDummyCreator : EntityDummyCreator, IDummyCreator<Newspaper>
    {
        CountryDummyCreator countryCreator = new CountryDummyCreator();
        CitizenDummyCreator citizenCreator = new CitizenDummyCreator();

        private Newspaper newspaper
        {
            get
            {
                return entity.Newspaper;
            }
            set
            {
                value.Entity = entity;
                value.ID = entity.EntityID;
                entity.Newspaper = value;
            }
        }
        public NewspaperDummyCreator()
        {
            createNewspaper();
        }

        protected void createNewspaper()
        {
            base.createEntity(EntityTypeEnum.Newspaper);
            newspaper = new Newspaper();
            newspaper.Owner = citizenCreator.Create().Entity;

            newspaper.Country = countryCreator.Create();
            newspaper.CountryID = newspaper.Country.ID;

        }

        public new Newspaper Create()
        {
            var temp = newspaper;
            createNewspaper();
            return temp;
        }
    }
}
