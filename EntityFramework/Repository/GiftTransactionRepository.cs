using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class GiftTransactionRepository : RepositoryBase<GiftTransaction, SociatisEntities>, IGiftTransactionRepository
    {
        public GiftTransactionRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
