using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Sociatis_Test_Suite.Persistents
{
    public class TestCurrencyPersistentRepository : CurrencyPersistentRepository
    {
        public override void Add(Currency t)
        {
            storage.Add(t);
        }
    }
}
