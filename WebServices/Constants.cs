
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public static class Constants
    {
        public const int ElectedCongressCandidatesByRegion = 2;
        public const decimal DefaultVat = 0.05m;
        public const decimal DefaultImportTax  = 0.1m;
        public const decimal DefaultExportTax = 0.1m;

        public const int MinimalVotingPercentage = 5;
        public const int MaximalVotingPercentage = 75;

        public const int ContractMinimumLength = 5;
        public const int ContractMaximumLength = 45;

        public const double PresidentCadenceMedalGold = 10;
        public const int PresidentCadenceDefaultLength = 30;

        public const double CongressCadenceMedalGold = 5;
        public const int CongressCadenceDefaultLength = 30;

        public const string DefenseSystemConstructionName = "Defense system construction - {0}";
        public const string HospitalConstructionName = "Hospital construction - {0}";
    }
}
