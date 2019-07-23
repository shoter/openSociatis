using Common.Operations;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WebServices.Helpers;
using WebServices.structs;
using WebServices.structs.Organisations;

namespace WebServices
{
    public class OrganisationService : IOrganisationService
    {
        IEntityService entityService;
        IOrganisationRepository organisationRepository;

        public OrganisationService(IEntityService entityService, IOrganisationRepository organisationRepository)
        {
            this.entityService = entityService;
            this.organisationRepository = organisationRepository;
        }

        public Organisation CreateOrganisation(CreateOrganisationParameters param)
        {

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, SociatisTransactionOptions.RepeatableRead))
            {
                var entity = entityService.CreateEntity(param.Name, EntityTypeEnum.Organisation);
                var organisation = new Organisation()
                {
                    Entity = entity,
                    OwnerID = param.OwnerID,
                    CountryID = param.CountryID
                };

                organisationRepository.Add(organisation);
                organisationRepository.SaveChanges();

                scope.Complete();
                return organisation;
            }
        }

        public MethodResult CanSeeInventory(Entity currentEntity, Citizen loggedCitizen, Organisation organisation)
        {
            if (organisation == null)
                return new MethodResult("Organisation does not exist!");

            var rights = GetOrganisationRights(currentEntity, loggedCitizen, organisation);

            if (rights.CanSeeInventory)
                return MethodResult.Success;

            return new MethodResult("You cannot do that!");
        }

        public MethodResult CanSeeWallet(Entity currentEntity, Citizen loggedCitizen, Organisation organisation)
        {
            if (organisation == null)
                return new MethodResult("Organisation does not exist!");

            var rights = GetOrganisationRights(currentEntity, loggedCitizen, organisation);

            if (rights.CanSeeWallet)
                return MethodResult.Success;

            return new MethodResult("You cannot do that!");
        }

        public OrganisationRights GetOrganisationRights(Entity currentEntity, Citizen loggedCitizen, Organisation organisation)
        {
            if (currentEntity.EntityID == organisation.ID)
                return new OrganisationRights(true);

            if (currentEntity.Is(EntityTypeEnum.Citizen))
            {
                if (organisation.OwnerID == loggedCitizen.ID)
                    return new OrganisationRights(true);

                if (organisation.Managers.Any(m => m.ID == loggedCitizen.ID))
                    return new OrganisationRights(true);
                
            }

            return new OrganisationRights(false);
        }
    }
}
