using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class CompanyDummyCreator : EntityDummyCreator, IDummyCreator<Company>
    {
        private RegionDummyCreator regionDummyCreator = new RegionDummyCreator();
        private Company company
        {
            get { return entity.Company; }
            set
            {
                value.Entity = entity;
                value.ID = entity.EntityID;
                entity.Company = value;
            }
        }

        public CompanyDummyCreator()
        {
            createCompany();
        }

        public new Company Create()
        {
            var _return = company;
            createCompany();
            return _return;
        }

        public CompanyDummyCreator SetOwner(Entity entity)
        {
            company.Owner = entity;
            company.OwnerID = entity.EntityID;
            entity.OwnedCompanies.Add(company);

            return this;
        }

        public CompanyDummyCreator SetRegion(Region region)
        {
            company.Region = region;
            company.RegionID = region.ID;
            region.Companies.Add(company);

            return this;
        }

        private void createCompany()
        {
            base.createEntity(EntityTypeEnum.Company);
            company = new Company();

            SetRegion(regionDummyCreator.Create());

        }
    }
}
