using Common.Extensions;
using Common.ForEnums;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Calculator
{
    public class CalculateProductionPointsViewModel
    {
        public List<SelectListItem> Products { get; set; }
        public List<SelectListItem> ResourceQualities { get; set; }
        [Required]
        public int? ProducedProductID { get; set; }
        [Required]
        [Range(0, 100)]
        public int HitPoints { get; set; } = 100;
        [Range(1, 5)]
        [Required]
        public int Quality { get; set; } = 1;
        [Required]
        [Range(0, double.MaxValue)]
        public double Skill { get; set; } = 4.0;
        [Required]
        [Range(0, double.MaxValue)]
        public double Distance { get; set; } = 9;
        [Required]
        [Range(0, 5.0)]
        public double Development { get; set; } = 1.0;
        [Range(0, int.MaxValue)]
        [Required]
        public int PeopleCount { get; set; } = 1;
        [Range(0, 4)]
        public int? ResourceQuality { get; set; }

        public double? Result { get; set; }

        public CalculateProductionPointsViewModel()
        {
            Products = new EnumSelectList<ProductTypeEnum>(x => x.ToHumanReadable().FirstUpper(), includeSelect: true);
            ResourceQualities = new EnumSelectList<ResourceFertilityEnum>(x => x.ToHumanReadable().FirstUpper(), includeSelect: true);
        }
    }
}