using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Test_Suite.Dummies
{
    public class HospitalDummyCreator : CompanyDummyCreator, IDummyCreator<Hospital>
    {
        Hospital hospital;
        public HospitalDummyCreator()
        {
            hospital = create();
        }

        Hospital create()
        {
            return new Hospital()
            {
                HealingEnabled = true,
                HealingPrice = 1m,
                HospitalNationalityHealingOptions = new List<HospitalNationalityHealingOption>(),
            };
        }



        public new Hospital Create()
        {
            var tmp = hospital;
            hospital = create();

            var company = base.Create();
            tmp.Company = company;
            tmp.CompanyID = company.ID;
            company.Hospital = tmp;

            return tmp;
        }
    }
}
