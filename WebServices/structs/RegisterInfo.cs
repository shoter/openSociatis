using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class RegisterInfo
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int CountryID { get; set; }
        public int RegionID { get; set; }
        public PlayerTypeEnum PlayerType { get; set; }
    }
}
