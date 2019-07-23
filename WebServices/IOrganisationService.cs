using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs;
using WebServices.structs.Organisations;

namespace WebServices
{
    public interface IOrganisationService
    {
        Organisation CreateOrganisation(CreateOrganisationParameters param);
        MethodResult CanSeeInventory(Entity currentEntity, Citizen loggedCitizen, Organisation organisation);

        MethodResult CanSeeWallet(Entity currentEntity, Citizen loggedCitizen, Organisation organisation);

        OrganisationRights GetOrganisationRights(Entity currentEntity, Citizen loggedCitizen, Organisation organisation);
    }
}
