using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.structs
{
    public class CitizenDom
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public CitizenDom(Citizen citizen)
        {
            ID = citizen.ID;
            Name = citizen.Entity.Name;
        }
    }
}
