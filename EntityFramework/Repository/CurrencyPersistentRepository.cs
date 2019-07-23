using Entities.enums;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CurrencyPersistentRepository : PersistentRepositoryBase<Currency>, ICurrencyRepository
    {
        public CurrencyPersistentRepository() : base(new PersistentListStorage<Currency>())
        {
        }

        public Currency Gold
        {
            get
            {
                return _Gold;
            }
        }

        private Currency _Gold = null;

        protected override void AfterLoad()
        {
            _Gold = First(c => c.ID == (int)CurrencyTypeEnum.Gold);
        }

        public override Currency GetById(int id)
        {
            return FirstOrDefault(c => c.ID == id);
        }

        protected override void LoadEntity(Currency persistentEntity, Currency dbEntity)
        {
            Load(x => x.Countries, persistentEntity, dbEntity);
        }
    }
}
