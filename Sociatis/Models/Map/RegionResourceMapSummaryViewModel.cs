using Entities;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Models.Map
{
    public class RegionResourceMapSummaryViewModel
    {
        public string ResourceName { get; set; }
        public string ResourceQuality { get; set; }

        public RegionResourceMapSummaryViewModel(Resource resource)
        {
            ResourceName = ((ResourceTypeEnum)resource.ResourceTypeID).ToHumanReadable();
            ResourceQuality = ((ResourceFertilityEnum)resource.ResourceQuality).ToHumanReadable();
        }
    }
}