using Common;
using Common.EncoDeco;
using Entities.Repository;
using Sociatis.Models;
using Sociatis.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Validators
{
    public class AccountValidator : Validator<LoginViewModel>
    {
        ICitizenRepository citizenRepository;
        IEntityService entityService;
        public AccountValidator(ModelStateDictionary ModelState, ICitizenRepository citizensRepository, IEntityService entityService) : base(ModelState)
        {
            this.citizenRepository = citizensRepository;
        }

        public override void Validate(LoginViewModel model, ValidatorAction action = ValidatorAction.Login)
        {
            var citizen = citizenRepository.FirstOrDefault(c => c.Entity.Name == model.Name);

            if(citizen == null)
            {
                AddError("Citizen does not exist!", () => model.Name);
            }
            else
            {
                var hash = SHA256.Encode(model.Password);
                if(hash != citizen.Password)
                {
                    AddError("Password does not match!", () => model.Password);
                }
            }
        }

        public void Validate(RegisterViewModel model, ValidatorAction action = ValidatorAction.Register)
        {
            if (model == null)
            {
                AddError("You provided no data");
                return;
            }

            if (entityService.IsNameTaken(model.Name))
            {
                AddError("Name is taken", () => model.Name);
            }

            if (entityService.IsSpecialName(model.Name))
            {
                AddError("This is special name. You cannot use it", () => model.Name);
            }

            if (citizenRepository.IsEmailAddressAllowed(model.Email) == false)
                AddError("Email address not registered for beta!");
            else if (citizenRepository.IsEmailAddressUsed(model.Email))
                AddError("Email address is already used!");

            
        }
    }
}
