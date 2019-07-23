using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Businesses
{
    public class Business
    {
        public int BusinessID { get; set; }
        public string BusinessName { get; set; }
        public BusinessTypeEnum BusinessType { get; set; }
        public string BusinessImgUrl { get; set; }
        public int? Messages { get; set; }
        public int? Warnings { get; set; }
        public int? UnreadMessages { get; set; }
        public int? UnreadWarnings { get; set; }
        public bool CanSwitchInto { get; set; }
    }
}
