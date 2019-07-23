using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class MilitaryProtectionPactOfferRepository : RepositoryBase<MilitaryProtectionPactOffer, SociatisEntities>, IMilitaryProtectionPactOfferRepository
    {
        public MilitaryProtectionPactOfferRepository(SociatisEntities context) : base(context)
        {
        }

        public bool AreAlreadyHaveMPP(int countryID, int otherCountryID)
        {
            return Any(o =>
            (o.FirstCountryID == countryID && o.SecondCountryID == otherCountryID)
            ||
            (o.FirstCountryID == otherCountryID && o.SecondCountryID == countryID));
        }
    }
}
