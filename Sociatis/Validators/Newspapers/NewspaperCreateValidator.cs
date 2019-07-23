using Entities;
using Sociatis.Models.Newspapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Validators.Newspapers
{
    public class NewspaperCreateValidator : Validator<CreateNewspaperViewModel>
    {
        private readonly Entity creator;
        public NewspaperCreateValidator(Entity creator)
        {
            this.creator = creator;
        }

        public NewspaperCreateValidator(ModelStateDictionary ModelState) : base(ModelState)
        {

        }
    }
}