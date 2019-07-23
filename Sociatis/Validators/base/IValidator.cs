using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis.Validators
{
    public interface IValidator
    {
        List<ValidationError> ValidationErrors { get; set; }
        bool IsValid { get; set; }
    }

    public interface IValidator<TEntity> : IValidator
        where TEntity : class
    {



        void Validate(TEntity Entity, ValidatorAction action = ValidatorAction.Undefined);
    }
}
