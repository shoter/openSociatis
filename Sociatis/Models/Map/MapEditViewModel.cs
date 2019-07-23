using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sociatis.Models.Map
{
    public class MapEditViewModel : MapViewModel
    {
        public List<SelectListItem> SelectListRegions { get; set; } = new List<SelectListItem>();
        private bool editable;

        public bool Editable
        {
            get { return editable; }
            set
            {
                Regions.ForEach(r => r.Editable = true);
                editable = value;
            }
        }


        public MapEditViewModel(List<Region> regions) : base(regions)
        {

            SelectListRegions.Add(new SelectListItem() { Value = null, Text = "None" });

            SelectListRegions.AddRange(regions
                .Select(r => new SelectListItem()
                {
                    Value = r.ID.ToString(),
                    Text = r.Name
                }));
        }
    }
}
