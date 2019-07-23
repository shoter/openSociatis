using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Extensions
{
    public static class CompanyExtension
    {
        public static ProductTypeEnum GetProducedProductType(this Company company)
        {
            return (ProductTypeEnum)company.ProductID;
        }
        public static EquipmentItem GetProducedProductItem(this Company company)
        {
            var stockItem = company.Entity.Equipment.EquipmentItems.FirstOrDefault(i => i.ProductID == company.ProductID);
            if(stockItem == null)
            {
                stockItem = new EquipmentItem()
                {
                    Amount = 0,
                    ProductID = company.ProductID,
                    Quality = company.Quality,
                    EquipmentID = company.Entity.EquipmentID.Value
                };

                company.Entity.Equipment.EquipmentItems.Add(stockItem);
            }

            return stockItem;
        }

        public static CompanyTypeEnum GetCompanyType(this Company company)
        {
            return (CompanyTypeEnum)company.CompanyTypeID;
        }

        /// <summary>
        /// Can return null
        /// </summary>
        public static CompanyEmployee GetCompanyEmployee(this Company company, Citizen citizen)
        {
            return company.CompanyEmployees.FirstOrDefault(employee => employee.CitizenID == citizen.ID);
        }


        public static bool Is(this Company company, CompanyTypeEnum companyType)
        {
            return company.CompanyTypeID == (int)companyType;
        }

        public static WorkTypeEnum GetWorkType(this Company company)
        {
            return (WorkTypeEnum)company.WorkTypeID;
        }
    }
}
