using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class MonetaryTransactionRepository : RepositoryBase<MonetaryTransaction, SociatisEntities>, IMonetaryTransactionRepository
    {
        public MonetaryTransactionRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
