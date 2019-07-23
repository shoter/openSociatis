using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CompanyRepository : RepositoryBase<Company, SociatisEntities>, ICompanyRepository
    {

        /* public override IQueryable<Company> Query
         {
             get
             {
                 return base.Query
                     .Include(c => c.Entity)
                     .Include(c => c.Entity.Wallet)
                     .Include(c => c.Region);
             }
         }*/

        public IQueryable<Company> GetNationalCompanies(int countryID)
        {
            return Where(company => company.OwnerID == countryID);
        }

        public CompanyRepository(SociatisEntities context) : base(context)
        {
        }

        public List<Product> GetProducts()
        {
            return context.Products.ToList();
        }

        public bool IsManager(int companyID, int entityID)
        {
            return context.CompanyManagers
                .Any(r => r.EntityID == entityID 
                  && r.CompanyID == companyID);
        }
    }
}
