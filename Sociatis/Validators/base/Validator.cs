using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Sociatis.Validators
{
    public abstract class Validator<TEntity> : IValidator<TEntity>
       where TEntity : class
    {
#if DEBUG
        private List<ValidationError> _D_ModelStateErrors;
        public List<ValidationError> D_ModelStateErrors
        {
            get
            {
                getModelStateErrors();
                return _D_ModelStateErrors;
            }
            set
            {
                _D_ModelStateErrors = value;
            }
        }

#endif
        public List<ValidationError> ValidationErrors { get; set; }
        public ModelStateDictionary ModelState { get; set; }
        public object Parent { get; set; }
        public string ModelStatePrefix { get; set; }
        private bool _IsValidated = true;
        public bool IsValid
        {
            set
            {
                _IsValidated = value;
            }
            get
            {
                if (ModelState != null)
                    return ModelState.IsValid && _IsValidated;
                return _IsValidated;
            }
        }

        public Validator(ModelStateDictionary ModelState)
            : this()
        {
            this.ModelState = ModelState;
#if DEBUG
            _D_ModelStateErrors = new List<ValidationError>();
#endif
        }
#if DEBUG
        private void getModelStateErrors()
        {
            var keys = ModelState.Keys.ToList();
            var vals = ModelState.Values.ToList();
            _D_ModelStateErrors.Clear();

            for (int i = 0; i < keys.Count; ++i)
            {
                if (vals[i].Errors.Count > 0)
                {
                    foreach (var error in vals[i].Errors)
                    {
                        _D_ModelStateErrors.Add(
                            new ValidationError(error.ErrorMessage, keys[i])
                            );
                    }
                }
            }
        }
#endif

        public Validator()
        {
            ValidationErrors = new List<ValidationError>();
        }

        public void LinkWithParent<T>(Validator<T> parent, string prefix)
            where T : class
        {
            this.ModelState = parent.ModelState;
            this.ValidationErrors = parent.ValidationErrors;
            this.ModelStatePrefix = prefix;
            this.Parent = parent;
        }

        /// <param name="field">name of the field</param>
        /// <returns>Returns true if given field have error</returns>
        public bool HasError(string field)
        {
            return ValidationErrors.Any(ve => ve.Field == field);
        }

        public bool HasError<T>(Expression<Func<T>> expr)
        {
            string field = ExpressionHelper.GetExpressionText(expr);
            if (Parent != null)
                field = "." + field;
            return HasError(field);
        }

        /// <summary>
        /// Add new Error to Validator which automatically invalidates the entity.
        /// </summary>
        /// <param name="message">Message error that will be added to validation Errors</param>
        protected void AddError(string message)
        {
            IsValid = false;
            ValidationErrors.Add(new ValidationError(message));
            ModelState?.AddModelError("", message);
        }

        protected void AddError(string message, string field)
        {
            IsValid = false;
            field = ModelStatePrefix + field;
            if (ModelState != null)
                ModelState.AddModelError(field, message);
            ValidationErrors.Add(new ValidationError(message, field));
        }

        /// <summary>
        /// Removes all errors linked with field in ModelState. It is worth noticing that is also erasing model state errors inside field
        /// </summary>
        protected void RemoveErrors(string field)
        {
            ModelState
                .Where(item => item.Key.StartsWith(field))
                .ToList()
                .ForEach(item => ModelState[item.Key].Errors.Clear());
        }

        /// <summary>
        /// Removes all errors linked with field in ModelState. It is worth noticing that is also erasing model state errors inside field
        /// </summary>
        protected void RemoveErrors<T>(Expression<Func<T>> expr)
        {
            RemoveErrors(ExpressionHelper.GetExpressionText(expr));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">Message to display as the error</param>
        /// <param name="expr">
        /// () => model.ID - valid usage
        /// () => model.Something.ID - invalid usage( will point to .ID not .Something.ID)
        /// </param>
        protected void AddError<T>(string message, Expression<Func<T>> expr)
        {
            string field = ExpressionHelper.GetExpressionText(expr);
            if (Parent != null)
                field = "." + field;
            AddError(message, field);
        }



        public override string ToString()
        {
            string str = "";
            foreach (var error in ValidationErrors)
            {
                str += error.Error;
                if (error != ValidationErrors.Last())
                    str += "\nl";
            }

            return str;
        }

        public virtual void Validate(TEntity model, ValidatorAction action = ValidatorAction.Undefined) { }
    }
}
