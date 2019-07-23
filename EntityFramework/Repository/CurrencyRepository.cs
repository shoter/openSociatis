using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CurrencyRepository : RepositoryBase<Currency, SociatisEntities>, ICurrencyRepository
    {
        IEnumerable<Currency> currencies = null;
        Mutex currencyLock = new Mutex();

        public CurrencyRepository(SociatisEntities context) : base(context)
        {
        }

        public override IEnumerable<Currency> GetAll()
        {
            if(currencies == null)
            {
                lock(currencyLock)
                {
                    if(currencies == null)
                    {
                        currencies = base.GetAll();
                    }
                }
            }

            return currencies;
        }

        public Currency Gold
        {
            get
            {
                return DbSet.First(c => c.Symbol == "Gold");
            }
        }
    }
}
