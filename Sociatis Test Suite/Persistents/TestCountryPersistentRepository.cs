using Entities.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Sociatis_Test_Suite.Persistents
{
    public class TestCountryPersistentRepository : CountryPersistentRepository
    {

        public override void Add(Country t)
        {
            storage.Add(t);
        }
    }
}
