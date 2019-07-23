using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CompanyManagerRepository : RepositoryBase<CompanyManager, SociatisEntities>, ICompanyManagerRepository
    {
        public CompanyManagerRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
