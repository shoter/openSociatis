using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class TransactionLogRepository : RepositoryBase<TransactionLog, SociatisEntities>, ITransactionLogRepository
    {
        public TransactionLogRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
