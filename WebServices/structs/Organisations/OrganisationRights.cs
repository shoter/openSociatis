using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs.Organisations
{
    public class OrganisationRights
    {
        public readonly bool CanSeeWallet;
        public readonly bool CanSeeInventory;
        public readonly bool CanSwitchInto;

        public bool AnyRights => CanSeeWallet || CanSeeInventory || CanSwitchInto;
        public bool FullRights => CanSeeWallet && CanSeeInventory && CanSwitchInto;

        public OrganisationRights(bool canSeeWallet, bool canSeeInventory, bool canSwitchInto)
        {
            CanSeeWallet = canSeeWallet;
            CanSeeInventory = canSeeInventory;
            CanSwitchInto = canSwitchInto;
        }

        public OrganisationRights(bool defaultRights) : this(defaultRights, defaultRights, defaultRights) { }
    }
}
