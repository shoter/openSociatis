using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface ICompanyFinanceService
    {
        MethodResult CanAccessFinances(Company company, Entity entity, Citizen loggedCitizen);
    }
}
