using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json
{
    public class JsonDebugErrorModel : JsonErrorModel
    {
#if JsonDebug
        public string ExceptionStack { get; set; }
        public string ExcpetionMessage { get; set; }
#endif
        public JsonDebugErrorModel(Exception e) : base()
        {
#if DEBUG
            ErrorMessage = e.Message;
#else
            ErrorMessage = "Error";
#endif
#if JsonDebug
            ExceptionStack = e.StackTrace;
            ExcpetionMessage = e.Message;
#endif

        }
    }
}