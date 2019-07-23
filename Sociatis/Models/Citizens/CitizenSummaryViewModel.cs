using Common.Converters;
using Entities;
using Entities.enums;
using Entities.Extensions;
using Entities.Models.Citizens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices;

namespace Sociatis.Models.Citizens
{
    public class CitizenSummaryViewModel
    {
        public string Name { get; set; }
        public bool EatingSafety { get; set; } = false;
        public bool Trained { get; set; }
        public bool CanWork { get; set; }
        public ImageViewModel Avatar { get; set; }

        public int CitizenID { get; set; }

        public int HitPoints { get; set; }

        public int Experience { get; set; }
        
        public double Strength { get; set; }

        public MoneyViewModel CountryMoney { get; set; }
        public MoneyViewModel AdminMoney { get; set; }

        public int? JobID { get; set; }

        public int ExperienceProgress { get; set; }
        public int NextExperienceLevel { get; set; }

        public string MilitaryRank { get; set; }
        public string NextMilitaryRank { get; set; }
        public int NextMilitaryRankProgress { get; set; }

        public int UnreadMessages { get; set; } = 0;
        public int UnreadWarnings { get; set; } = 0;

        public int CountryID { get; set; }

        public bool CanHeal { get; set; } = false;
        public double HealCost { get; set; }
        public bool FreeHealing { get; set; }
        public string HealCurrency { get; set; }
        public int HospitalID { get; set; }

        public CitizenSummaryViewModel(CitizenSummaryInfo info, ICitizenService citizenService)
        {
            Name = info.Name;
            CitizenID = info.ID;
            CanWork = info.CanWork;
            HitPoints = info.HitPoints;
            Experience = info.Experience;
            Trained = info.Trained;
            Strength = info.Strength;
            Avatar = new ImageViewModel(info.AvatarUrl);
            JobID = info.JobID;

            EatingSafety = info.EatingSafety;

            CountryMoney = new MoneyViewModel(info.CountryCurrencyID, (decimal)info.CountryMoneyAmount);
            AdminMoney = new MoneyViewModel(CurrencyTypeEnum.Gold, (decimal)info.GoldAmount);
            CountryID = info.CountryID;

            NextExperienceLevel = citizenService.CalculateExperienceForNextLevel(info.Level);

            ExperienceProgress = (int)(info.Experience / (double)NextExperienceLevel * 100.0);

            int militaryRank = (int)info.MilitaryRank;
            int nextMilitaryRank = MilitaryRankEnumExtensions.CalculateNextMilitaryRankNeeded(militaryRank) ?? militaryRank;
            MilitaryRank = militaryRank.ConvertToBasicUnits();
            NextMilitaryRank = nextMilitaryRank.ConvertToBasicUnits();
            NextMilitaryRankProgress = (int)(militaryRank / (double)nextMilitaryRank * 100.0);

            UnreadMessages = info.UnreadMessages;
            UnreadWarnings = info.UnreadWarnings;

            if (HitPoints < 90 && info.CanHeal)
            {
                CanHeal = info.CanHeal;

                HealCurrency = CountryMoney.Symbol;
                HospitalID = info.HospitalID.Value;
                FreeHealing = info.HealingCost.HasValue == false;
                if (FreeHealing == false)
                {
                    HealCost = (double)info.HealingCost.Value;
                }
            }

        }
    }
}
