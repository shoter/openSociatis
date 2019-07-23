using Entities.enums;
using Entities.Repository;
using Sociatis.Helpers;
using Sociatis.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Models.War
{
    public class StartBattleViewModel
    {
        public WarInfoViewModel Info { get; set; }
        public List<SelectListItem> ConquerableRegions { get; set; } = new List<SelectListItem>();
        public int SelectedRegionID { get; set; }

        public StartBattleViewModel() { }

        public StartBattleViewModel(Entities.War war, IWarRepository warRepository, IWarService warSerivce)
        {
            Info = new WarInfoViewModel(war, warRepository, warSerivce);

            var warSide = warSerivce.GetWarSide(war, SessionHelper.CurrentEntity);
            ConquerableRegions.Add(new SelectListItem()
            {
                Value = "null",
                Text = "-- Select region --"
            });

            ConquerableRegions.AddRange(warRepository.GetAttackableRegions(war.ID, warSide == WarSideEnum.Attacker)
                .Select(r => new { Name = r.Name, ID = r.ID })
                .ToList()
                .Select(r => new SelectListItem() { Value = r.ID.ToString(), Text = r.Name })
                .ToList());
        }
    }
}