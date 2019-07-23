using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json
{
    public class JsonWarningModel : JsonModelBase
    {
        public string Message { get; set; }

        public JsonWarningModel()
        {
            Status = JsonStatusEnum.Warning;
        }

        public JsonWarningModel(string message) : this()
        {
            Message = message;
        }
    }
}