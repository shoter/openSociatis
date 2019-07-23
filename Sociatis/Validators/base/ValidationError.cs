using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Validators
{
    public class ValidationError
    {
        public ValidationError(string Error, string Field = null)
        {
            this.Error = Error;
            this.Field = Field;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", Field, Error);
        }

        public string Field { get; set; }
        public string Error { get; set; }
    }
}