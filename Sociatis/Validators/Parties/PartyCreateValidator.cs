using Entities.enums;
using Entities.Extensions;
using Sociatis.Helpers;
using Sociatis.Models.Party;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Validators.Parties
{
    public class PartyCreateValidator : Validator<CreatePartyViewModel>
    {
        private readonly IEntityService entityService;
        public PartyCreateValidator(ModelStateDictionary ModelState, IEntityService entityService) : base(ModelState)
        {
            this.entityService = entityService;
        }

        public override void Validate(CreatePartyViewModel model, ValidatorAction action = ValidatorAction.Undefined)
        {
            var currentEntity = SessionHelper.CurrentEntity;

            if (currentEntity.Is(EntityTypeEnum.Citizen) == false)
                AddError("You must be a citizen to create a party!");

            if (currentEntity.Citizen.PartyMember != null)
                AddError("You cannot be in another party to create party!");

            if (entityService.IsNameTaken(model.Name))
                AddError("Name is already used!");

            if (entityService.IsSpecialName(model.Name))
                AddError("This is special name. You cannot use it");
        }
    }
}