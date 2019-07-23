using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CompanyEmployeeRepository : RepositoryBase<CompanyEmployee, SociatisEntities>, ICompanyEmployeeRepository
    {
        public CompanyEmployeeRepository(SociatisEntities context) : base(context)
        {
        }

        public override CompanyEmployee GetById(int id)
        {
            return context.CompanyEmployees
                .FirstOrDefault(ce => ce.CitizenID == id);
        }

    }
}
