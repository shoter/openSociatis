using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices.structs
{
    public class CreateSessionParameters
    {
        public int CitizenID { get; set; }
        public string IP { get; set; }
        public bool RememberMe { get; set; }
    }
}
