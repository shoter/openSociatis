using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json
{
    public class JsonDataModel : JsonSuccessModel
    {
        public object Data { get; set; }
        public JsonDataModel(object data) : base()
        {
            Data = data;
        }
    }
}