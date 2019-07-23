using Common.Operations;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServices
{
    public interface IMPPService
    {
        MethodResult CanOfferMPP(Entity entity, Country proposingCountry, Country secondCountry, int days);
        decimal CalculateMPPCost(int days);

        MethodResult CanAcceptMPP(MilitaryProtectionPactOffer offer, Entity entity);
            MethodResult CanRefuseMPP(MilitaryProtectionPactOffer offer, Entity entity);

        void RefuseMPP(MilitaryProtectionPactOffer offer);
        void AcceptMPP(MilitaryProtectionPactOffer offer);

        MethodResult CanOfferMPP(Entity entity, Country proposingCountry);
        void OfferMPP(Entity entity, Country proposingCountry, Country secondCountry, int days);

        IEnumerable<Country> GetListOfCountriesWhereCitizenCanManageMPPs(Citizen citizen);

        void CancelMPP(MilitaryProtectionPactOffer offer);
        MethodResult CanCancelMPP(MilitaryProtectionPactOffer offer, Entity entity);

        void ProcessDayChange(int newDay);
    }
}
