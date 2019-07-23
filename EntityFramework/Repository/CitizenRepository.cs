using Common.EncoDeco;
using Common.EntityFramework;
using Entities.enums;
using Entities.Models.Citizens;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class CitizenRepository : RepositoryBase<Citizen, SociatisEntities>, ICitizenRepository
    {
        public CitizenRepository(SociatisEntities context) : base(context)
        { }
        public override IQueryable<Citizen> Query
        {
            get
            {
                return base.Query
                    .Include(c => c.Entity)
                    .Include(c => c.Entity.Wallet);
            }
        }

        public Citizen GetByName(string name)
        {
            name = name.ToLower().Trim();
            return FirstOrDefault(c => c.Entity.Name.ToLower() == name);
        }

        public Citizen GetByEmail(string email)
        {
            email = email.Trim();

            var citizen = FirstOrDefault(c => c.Email == email);

            if (citizen == null)
                citizen = FirstOrDefault(c => c.Email.ToLower() == email.ToLower());

            return citizen;

        }

        public Citizen GetByCredentials(string username, string password)
        {
            var hash = SHA256.Encode(password);
            return FirstOrDefault(c => c.Entity.Name == username && c.Password == hash);
        }
        public CitizenSummaryInfo GetSummary(int citizenID)
        {
            int goldID = (int)CurrencyTypeEnum.Gold;
            int breadID = (int)ProductTypeEnum.Bread;

            return (from citizen in context.Citizens.Where(c => c.ID == citizenID)
                    join entity in context.Entities on citizen.ID equals entity.EntityID
                    join employee in context.CompanyEmployees on citizen.ID equals employee.CitizenID into employee
                    join region in context.Regions on citizen.RegionID equals region.ID
                    join country in context.Countries on region.CountryID equals country.ID
                    join gold in context.WalletMoneys.Where(wm => wm.CurrencyID == goldID) on entity.WalletID equals gold.WalletID into golds
                    join countryMoney in context.WalletMoneys on
                    new { WalletID = entity.WalletID, CurrencyID = country.CurrencyID } equals
                    new { WalletID = countryMoney.WalletID, CurrencyID = countryMoney.CurrencyID } into countryMoneys
                    join warning in context.Warnings.Where(w => w.Unread) on citizen.ID equals warning.EntityID into warnings
                    join message in context.MailboxMessages.Where(m => m.Unread) on citizen.ID equals message.Viewers_EntityID into messages
                    join bread in context.EquipmentItems.Where(i => i.ProductID == breadID) on entity.EquipmentID equals bread.EquipmentID into breads
                    join hospital in context.Hospitals.Where(h => h.HealingEnabled) on region.NationalHospitalID equals hospital.CompanyID into hospitals
                    join nationaHealingOption in context.HospitalNationalityHealingOptions on
                   new { HospitalID = hospitals.Select(h => (int?)h.CompanyID).FirstOrDefault() ?? -1, CountryID = country.ID }
                   equals
                   new { nationaHealingOption.HospitalID, nationaHealingOption.CountryID } 
                   into nationalHealingOptions
                    select new CitizenSummaryInfo()
                    {
                        AvatarUrl = entity.ImgUrl,
                        CanWork = citizen.Worked == false,
                        CountryCurrencyID = country.CurrencyID,
                        CountryID = country.ID,
                        EatingSafety = breads.Any(),
                        CountryMoneyAmount = (double)(countryMoneys.FirstOrDefault() == null ? 0m : countryMoneys.FirstOrDefault().Amount),
                        Experience = citizen.Experience,
                        GoldAmount = (double)(golds.FirstOrDefault() == null ? 0m : golds.FirstOrDefault().Amount),
                        HitPoints = citizen.HitPoints,
                        ID = citizen.ID,
                        JobID = employee.Select(e => (int?)e.CompanyID).FirstOrDefault(),
                        Level = citizen.ExperienceLevel,
                        MilitaryRank = (double)citizen.MilitaryRank,
                        Name = entity.Name,
                        Strength = (double)citizen.Strength,
                        Trained = citizen.Trained,
                        UnreadMessages = messages.Count(),
                        UnreadWarnings = warnings.Count(),
                        CanHeal = hospitals.Any() && citizen.UsedHospital == false,
                        HealingCost =
                        nationalHealingOptions.Any() ? nationalHealingOptions.Select(opt => opt.HealingPrice).FirstOrDefault() :
                        hospitals.Select(h => h.HealingPrice).FirstOrDefault(),
                        HospitalID = hospitals.Select(h => h.CompanyID).FirstOrDefault()
                    }).First();




            //join country in context.Countries on 

        }


        public bool IsEmailAddressAllowed(string address)
        {
            address = address?.Trim()?.ToLower() ?? "";

            return context.AllowedEmails.Any(e => e.Email.ToLower() == address);
        }

        public bool IsEmailAddressUsed(string address)
        {
            address = address.Trim().ToLower();

            return Any(c => c.Email.ToLower() == address);
        }

        private void test(int a, out int b)
        {
            b = 123;
        }

        public int GetActivePlayerCount(int currentDay)
        {
            return Where(c => currentDay - c.LastActivityDay <= 2).Count();
        }


    }
}


