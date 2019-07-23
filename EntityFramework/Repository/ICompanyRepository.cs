using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        List<Product> GetProducts();

        bool IsManager(int companyID, int entityID);

        IQueryable<Company> GetNationalCompanies(int countryID);
    }
}
