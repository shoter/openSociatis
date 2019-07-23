using Entities.enums;
using Entities.Models.Constructions;
using Sociatis.Helpers;
using Sociatis.Models.Entit;
using System;
using WebServices;

namespace Sociatis.Models.Constructions
{
    public class NationalConstructionItemViewModel
    {
        public int RegionID { get; set; }
        public string RegionName { get; set; }
        public int ConstructionID { get; set; }
        public SmallEntityAvatarViewModel Avatar { get; set; }
        public double Progress { get; set; }
        public int Quality { get; set; }

        public NationalConstructionItemViewModel(NationalConstruction construction, IConstructionService constructionService)
        {
            RegionID = construction.RegionID;
            RegionName = construction.RegionName;
            ConstructionID = construction.ConstructionID;
            Avatar = new SmallEntityAvatarViewModel(construction.ConstructionID, construction.ConstructionName, Images.UnderConstruction.Path)
                .DisableNameInclude();

            Quality = construction.Quality;
            Progress = ((double)construction.Progress / constructionService.GetProgressNeededToBuild(construction.ProductType, construction.Quality)) * 100.0; ;
            Progress = Math.Round(Progress, 1);
            
        }
    }
}
