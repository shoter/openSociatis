using Common.ForEnums;
using Entities.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils.Mvc;

namespace Sociatis.Models.Calculator
{
    public class CalculateFuelCostViewModel
    {
        public List<SelectListItem> Products { get; set; }
        public int ProductID { get; set; }
        public double? Distance { get; set; }
        [Range(1, 5)]
        [Required]
        public int Quality { get; set; }
        [Range(1, int.MaxValue)]
        [Required]
        public int Amount { get; set; }

        public List<SelectListItem> Regions { get; set; }

        public int? StartRegionID { get; set; }
        public int? EndRegionID { get; set; }
        public bool UseRegions { get; set; }

        public int? Result { get; set; }

        public CalculateFuelCostViewModel()
        {
            Products = new EnumSelectList<ProductTypeEnum>(x => x.ToHumanReadable(), includeSelect: true);
            Regions = new CustomSelectList()
                .AddSelect()
                .AddItems(Persistent.Regions.GetAll(), r => r.ID, r => r.Name);

        }
    }
}