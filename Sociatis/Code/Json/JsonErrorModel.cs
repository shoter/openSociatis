using Common.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json
{
    public class JsonErrorModel : JsonModelBase
    {
        public string ErrorMessage { get; set; }


        public JsonErrorModel()
        {
            Status = JsonStatusEnum.Error;
        }

        public JsonErrorModel(string errorMessage) : this()
        {
            ErrorMessage = errorMessage;
        }

        public JsonErrorModel(MethodResult result) : this()
        {
            ErrorMessage = "";

            for(int i = 0; i < result.Errors.Count;++i)
            {
                ErrorMessage += result.Errors[i];
                if (i != result.Errors.Count - 1)
                    ErrorMessage += "<br/>";
            }
        }


    }
}