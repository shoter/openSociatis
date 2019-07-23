using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class EmployeeExtension
    {
        public static JobTypeEnum GetJobType(this CompanyEmployee employee)
        {
            if (employee.JobContractID != null)
                return JobTypeEnum.Contracted;
            return JobTypeEnum.Normal;
        }
        public static double GetWorkSkill(this CompanyEmployee employee)
        {
            return employee.Citizen.GetWorkSkill((WorkTypeEnum)employee.Company.WorkTypeID);
        }
    }
}
