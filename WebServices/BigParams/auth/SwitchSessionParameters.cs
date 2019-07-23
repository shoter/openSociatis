using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.BigParams.auth
{
    public class SwitchSessionParameters
    {
        public int? PreviousEntityID { get; set; }
        public int EntityID { get; set; }
        public string IP { get; set; }
        public Session OldSession { get; set; }
    }
}
