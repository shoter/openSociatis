using Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public interface IMilitaryProtectionPactOfferRepository : IRepository<MilitaryProtectionPactOffer>
    {
        bool AreAlreadyHaveMPP(int countryID, int otherCountryID);
    }
}
