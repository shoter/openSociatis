using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.structs.Hospitals;

namespace WebServices
{
    public interface IHospitalService 
    {
        void HealCitizen(Citizen citizen, Hospital hospital);
        MethodResult CanHealCitizen(Citizen citizen, Hospital hospital);

        MethodResult CanManageHospital(Entity entity, Hospital hospital);

        void ReserveNamesForNationalHospitals();
        void UpdateHospital(Hospital hospital, decimal? healingPrice, bool healingEnabled);

        string GenerateNameForHospital(Region region);
        void UpdateHospitalNationalityOptions(Hospital hospital, IEnumerable<UpdateHospitalNationalityOption> options);

    }
}
