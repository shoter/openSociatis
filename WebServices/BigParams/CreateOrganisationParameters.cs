using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class CreateOrganisationParameters
    {
        public string Name { get; set; }
        public int CountryID { get; set; }
        public int OwnerID { get; set; }
    }
}
