using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.enums;

namespace WebServices
{
    public interface IEmbargoService
    {
        double GetEmbargoCost(Embargo embargo);
        double GetEmbargoCost(Country declareCountry, Country embargoedCountry);
        bool CanCancelEmbargo(Embargo embargo, Entity entity);
        bool CanUpkeepEmbargo(double cost, int walletID);
        bool CanUpkeepEmbargo(Country issuingCountry, Country embargoedCountry);
        void CancelEmbargo(Embargo embargo);
        void ProcessDayChange(int currentDay);
        TransactionResult MakeEmbargoCostTransaction(Embargo embargo, bool useSqlTransaction = false);
        void DeclareEmbargo(Country declaringCountry, Country embargoedCountry);
        bool CanDeclareEmbargo(Country declaringCountry, Country embargoedCountry, Entity issuingEntity);
        bool CanDeclareEmbargo(Country declaringCountry, Entity issuingEntity);
    }
}
