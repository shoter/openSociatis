using Entities.Repository;
using Sociatis.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sociatis.Validators.Messages
{
    public class SendMessageValidator : Validator<SendMessageViewModel>
    {
        IEntityRepository entityRepository;

        public SendMessageValidator(ModelStateDictionary ModelState, IEntityRepository entityRepository) : base(ModelState)
        {
            this.entityRepository = entityRepository;
        }

        public override void Validate(SendMessageViewModel model, ValidatorAction action = ValidatorAction.Undefined)
        {
            if(model.RecipientID.HasValue)
            {
                bool exists = entityRepository.Any(e => e.EntityID == model.RecipientID);

                if (!exists)
                    AddError("Recipient does not exists", () => model.RecipientName);
            }
            else
            {
                bool exists = entityRepository.Any(e => e.Name.ToLower() == model.RecipientName.ToLower());

                if (!exists)
                    AddError("Recipient does not exists", () => model.RecipientName);
            }
        }
    }
}
