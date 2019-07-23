using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ContractRepository : RepositoryBase<JobContract, SociatisEntities>, IContractRepository
    {
        public ContractRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
