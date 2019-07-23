using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json
{
    public class JsonSuccessModel : JsonModelBase
    {
        public string Message { get; set; }

        public JsonSuccessModel()
        {
            Status = JsonStatusEnum.Success;
        }

        public JsonSuccessModel(string message) : this()
        {
            Message = message;
        }
    }
}