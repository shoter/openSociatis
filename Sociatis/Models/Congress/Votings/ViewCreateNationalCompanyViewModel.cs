using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.Congress.Votings
{
    public class ViewCreateNationalCompanyViewModel : ViewVotingBaseViewModel
    {
        public string CompanyName { get; set; }
        public string ShopDomain { get; set; }
        public string RegionName { get; set; }

        public ViewCreateNationalCompanyViewModel(Entities.CongressVoting voting, bool isPlayerCongressman, bool canVote)
        :base(voting, isPlayerCongressman, canVote)
    {
            CompanyName = voting.Argument1;
            int regionID = Convert.ToInt32(voting.Argument2);
            ProductTypeEnum productType = (ProductTypeEnum)Convert.ToInt32(voting.Argument3);

            var region = Persistent.Regions.GetById(regionID);
            RegionName = region.Name;


            if (productType == ProductTypeEnum.MedicalSupplies)
            {
                var hospitalService = DependencyResolver.Current.GetService<IHospitalService>();
                CompanyName = hospitalService.GenerateNameForHospital(region);
            }

            switch (productType)
            {
                case ProductTypeEnum.SellingPower:
                    ShopDomain = "It will be a shop.";
                    break;
                default:
                    ShopDomain = $"It will produce {productType.ToHumanReadable()}.";
                    break;
            }

        }
    }
}