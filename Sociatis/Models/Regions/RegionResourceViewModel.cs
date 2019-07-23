using Entities;
using Entities.enums;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Regions
{
    public class RegionResourceViewModel
    {
        public string ResourceName { get; set; }
        public ImageViewModel ResourceImage { get; set; }
        public ResourceFertilityEnum ResourceQuality { get; set; }

        public RegionResourceViewModel(Resource resource)
        {
            var resourceType = (ResourceTypeEnum)resource.ResourceTypeID;
            ResourceName = resourceType.ToHumanReadable();
            ResourceQuality = (ResourceFertilityEnum)resource.ResourceQuality;
            ResourceImage = Images.GetResourceImage(resourceType).VM;


        }
    }
}