using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs
{
    class SessionDom
    {
        public int ID { get; set; }
        public string Cookie { get; set; }
        public int CitizenID { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool RememberMe { get; set; }
        
    }
}
