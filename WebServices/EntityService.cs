using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.enums;
using Entities.Repository;
using Entities.Extensions;
using System.Text.RegularExpressions;
using Common.Extensions;
using WebServices.Helpers;
using System.Web.Mvc;
using Common.Operations;

namespace WebServices
{
    public class EntityService : IEntityService
    {
        private readonly IEntityRepository entityRepository;
        private readonly IReservedEntityNameRepository reservedEntityNameRepository;

        public EntityService(IEntityRepository entitiesRepository, IReservedEntityNameRepository reservedEntityNameRepository)
        {
            this.entityRepository = entitiesRepository;
            this.reservedEntityNameRepository = reservedEntityNameRepository;
        }

        public Entity CreateEntity(string name, EntityTypeEnum entityType)
        {
            Wallet wallet = new Wallet();
            Equipment equipment = new Equipment();

            //name can be null - special entities i.e constructions
            name = name?.Trim();

            Entity entity = new Entity()
            {
                EntityTypeID = (int)entityType,
                Wallet = wallet,
                Name = name,
                Equipment = equipment,
                CreationDay = GameHelper.CurrentDay
            };

            entityRepository.Add(entity);
            entityRepository.SaveChanges();

            return entity;
        }

        public bool IsNameTaken(string name)
        {
            name = name.ToLower().Trim().MultipleSpaceRemove();

            if (entityRepository.Any(entity =>
             entity.Name.ToLower().Trim() == name))
                return true;

            return reservedEntityNameRepository.Any(r =>
                     r.Name.ToLower().Trim() == name);
        }

        public bool IsSpecialName(string name)
        {
            if (name.StartsWith(Constants.DefenseSystemConstructionName.FormatString("")))
                return true;
            if (name.StartsWith(Constants.HospitalConstructionName.FormatString("")))
                return true;
            return false;
        }



        public bool CanChangeInto(Entity currentEntity, Entity desiredEntity, Citizen loggedCitizen)
        {
            switch(desiredEntity.GetEntityType())
            {
                case EntityTypeEnum.Citizen:
                    return canChangeIntoCitizen(currentEntity, desiredEntity.Citizen, loggedCitizen);
                case EntityTypeEnum.Company:
                    return canChangeIntoCompany(currentEntity, desiredEntity.Company, loggedCitizen);
                case EntityTypeEnum.Party:
                    return canChangeIntoParty(currentEntity, desiredEntity.Party, loggedCitizen);
                case EntityTypeEnum.Organisation:
                    return canChangeIntoOrganisation(currentEntity, desiredEntity.Organisation, loggedCitizen);
                case EntityTypeEnum.Country:
                    return canChangeIntoCountry(currentEntity, desiredEntity.Country, loggedCitizen);
                case EntityTypeEnum.Newspaper:
                    return canChangeIntoNewspaper(currentEntity, desiredEntity.Newspaper, loggedCitizen);
                case EntityTypeEnum.Hotel:
                    return canChangeIntoHotel(currentEntity, desiredEntity.Hotel, loggedCitizen);
                default:
                    throw new NotImplementedException();
            }
        }

        private bool canChangeIntoHotel(Entity currentEntity, Hotel hotel, Citizen loggedCitizen)
        {
            var entityType = currentEntity.GetEntityType();
            if (entityType == EntityTypeEnum.Hotel)
                return false;

            if (entityType != EntityTypeEnum.Citizen && entityType != EntityTypeEnum.Organisation)
                return false;

            var service = DependencyResolver.Current.GetService<IHotelService>();

            var rights = service.GetHotelRigths(hotel, currentEntity);

            return rights.CanSwitchInto;
        }

        private bool canChangeIntoNewspaper(Entity currentEntity, Newspaper newspaper, Citizen loggedCitizen)
        {
            var entityType = currentEntity.GetEntityType();
            if (entityType == EntityTypeEnum.Newspaper)
                return false;

            if(newspaper.OwnerID != currentEntity.EntityID)
                return false;

            return true;
        }

        private bool canChangeIntoCountry(Entity currentEntity, Country country, Citizen loggedCitizen)
        {
            if (currentEntity.GetEntityType() != EntityTypeEnum.Citizen)
                return false;

            if (country.PresidentID != loggedCitizen.ID)
                return false;

            return true;
        }

        private bool canChangeIntoOrganisation(Entity currentEntity, Organisation organisation, Citizen loggedCitizen)
        {
            if (currentEntity.GetEntityType() != EntityTypeEnum.Citizen && (currentEntity.GetEntityType() != EntityTypeEnum.Company))
                return false;

            List<int> managersIDs = new List<int>() { organisation.OwnerID.Value };

            managersIDs.AddRange(organisation.Managers.Select(m => m.ID).ToList());

            return managersIDs.Contains(loggedCitizen.ID);
        }

        private bool canChangeIntoParty(Entity currentEntity, Party party, Citizen loggedCitizen)
        {
            if (currentEntity.GetEntityType() != EntityTypeEnum.Citizen)
                return false;

            var managersIDs = party.PartyMembers
                .Where(pm => pm.PartyRoleID >= (int)PartyRoleEnum.Manager)
                .Select(pm => pm.CitizenID)
                .ToList();


            return managersIDs.Contains(loggedCitizen.ID);
        }

        private bool canChangeIntoCompany(Entity currentEntity, Company company, Citizen loggedCitizen)
        {
            var companyService = DependencyResolver.Current.GetService<ICompanyService>();
            if (currentEntity.GetEntityType() != EntityTypeEnum.Citizen && currentEntity.GetEntityType() != EntityTypeEnum.Organisation)
                return false;

            var rights = companyService.GetCompanyRights(company, currentEntity, loggedCitizen);

            if (rights.CanSwitch == false)
                return false;

            return true;
        }

        private bool canChangeIntoCitizen(Entity currentEntity, Citizen citizen, Citizen loggedCitizen)
        {
            if (currentEntity.GetEntityType() == EntityTypeEnum.Citizen)
                return false;

            if (citizen.ID != loggedCitizen.ID)
                return false;

            return citizen.ID == loggedCitizen.ID;
        }

        public bool Exists(string name)
        {
            return entityRepository.Any(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        
    }
}
