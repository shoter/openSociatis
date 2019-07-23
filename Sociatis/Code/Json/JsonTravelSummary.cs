using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code.Json
{
    public class JsonTravelSummary : JsonPartialModel
    {
        public bool CanTravel { get; set; }

        public JsonTravelSummary(string content, bool canTravel) : base(content)
        {
            CanTravel = canTravel;
        }
    }
}