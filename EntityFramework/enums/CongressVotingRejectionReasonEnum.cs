using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.enums
{
    public enum CongressVotingRejectionReasonEnum
    {
        NotEnoughVotes = 0,
        NotEnoughGoldToConstructCompany = 1,
        RegionIsNotYoursConstructCompany = 2,
        CompanyIsNotYoursRemoveCompany = 3,
        AlreadyManagerAssignManager = 4,
        CompanyOutsideNationalBordersTransferMoney = 5,
        NotEnoughCashTransferMoney = 6
    }

    public static class CongressVotingRejectionReasonEnumExtensions
    {
        public static string ToHumanReadable(this CongressVotingRejectionReasonEnum rejectionReason)
        {
            switch (rejectionReason)
            {
                case CongressVotingRejectionReasonEnum.NotEnoughVotes:
                    return "not enough votes";
                case CongressVotingRejectionReasonEnum.NotEnoughGoldToConstructCompany:
                    return "not enough gold to construct company";
                case CongressVotingRejectionReasonEnum.RegionIsNotYoursConstructCompany:
                    return "region has been lost and you are unable to build a company there!";
                case CongressVotingRejectionReasonEnum.CompanyIsNotYoursRemoveCompany:
                    return "company is not longer in posess of your country!";
                case CongressVotingRejectionReasonEnum.AlreadyManagerAssignManager:
                    return "citizen is already manager of this party!";
                case CongressVotingRejectionReasonEnum.CompanyOutsideNationalBordersTransferMoney:
                    return "this company is outside national borders!";
                case CongressVotingRejectionReasonEnum.NotEnoughCashTransferMoney:
                    return "your country does not have enough cash!";
            }

            throw new NotImplementedException();
        }
    }
}
