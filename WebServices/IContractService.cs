using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IContractService
    {
        void ProcessDayChange();
        /// <summary>
        /// It does nothing to non-contract jobs
        /// </summary>
        /// <param name="employee"></param>
        void AddEmployeeAbusement(CompanyEmployee employee);
        /// <summary>
        /// It does nothing to non-contract jobs
        /// </summary>
        /// <param name="employee"></param>
        void AddCompanyAbusement(CompanyEmployee employee);

        void InformatAboutCompanyAbusement(Company company, CompanyEmployee employee);
        void InformAboutEmployeeAbusement(Company company, CompanyEmployee employee);
    }
}
