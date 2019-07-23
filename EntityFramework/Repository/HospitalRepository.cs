using Common.EntityFramework;
using Entities.Models.Hospitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class HospitalRepository : RepositoryBase<Hospital, SociatisEntities>, IHospitalRepository
    {
        public HospitalRepository(SociatisEntities context) : base(context)
        {
        }

        public HealingPrice GetHealingPrice(int hospitalID, int citizenID)
        {
            return (from hospital in context.Hospitals.Where(h => h.CompanyID == hospitalID)
                    join citizen in context.Citizens on citizenID equals citizen.ID
                    join company in context.Companies on hospital.CompanyID equals company.ID
                    join region in context.Regions on company.RegionID equals region.ID
                    join country in context.Countries on region.CountryID equals country.ID
                    join healingPrice in context.HospitalNationalityHealingOptions.Where(o => o.HospitalID == hospitalID) on citizen.CitizenshipID equals healingPrice.CountryID into healingPrices

                    select new HealingPrice()
                    {
                        CurrencyID = country.CurrencyID,
                        Cost = healingPrices.Any() ? healingPrices.Select(h => h.HealingPrice).FirstOrDefault() : hospital.HealingPrice
                    }).First();
        }

        public void SetHealingPrice(int hospitalID, int countryID, decimal? price)
        {
            var existing = context.HospitalNationalityHealingOptions.Find(hospitalID, countryID);
            if (existing == null)
            {
                AddNationalityHealingOption(hospitalID, countryID, price);
            }
            else
            {
                existing.HealingPrice = price;
            }
        }

        public void AddNationalityHealingOption(int hospitalID, int countryID, decimal? price)
        {
            context.HospitalNationalityHealingOptions.Add(new HospitalNationalityHealingOption()
            {
                HospitalID = hospitalID,
                CountryID = countryID,
                HealingPrice = price
            });
        }

        public HospitalManage GetHospitalManageInfo(int hospitalID)
        {
            return (from hospital in Where(h => h.CompanyID == hospitalID)
                    join nationalityOption in context.HospitalNationalityHealingOptions on hospital.CompanyID equals nationalityOption.HospitalID into nationalityOptions
                    join company in context.Companies on hospital.CompanyID equals company.ID
                    join region in context.Regions on company.RegionID equals region.ID
                    join country in context.Countries on region.CountryID equals country.ID
                    select new HospitalManage()
                    {
                        HealingEnabled = hospital.HealingEnabled,
                        HealingPrice = hospital.HealingPrice,
                        CurrencyID = country.CurrencyID,
                        hospitalManageNationalityHealingOptions = nationalityOptions.Select(o => new HospitalManageNationalityHealingOption()
                        {
                            CountryID = o.CountryID,
                            HealingPrice = o.HealingPrice
                        }).ToList()
                    }).First();
        }
    }
}
