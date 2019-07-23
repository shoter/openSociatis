using Common.DataTables;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.Best;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    public class BestController : ControllerBase
    {
        private readonly ICitizenRepository citizenRepository;

        public BestController(IPopupService popupService, ICitizenRepository citizenRepository) : base(popupService)
        {
            this.citizenRepository = citizenRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Best/Citizens")]
        [HttpGet]
        public ActionResult BestCitizens()
        {
            var vm = new BestCitizensViewModel();
            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Best/CitizensAjax")]
        [HttpPost]
        [AjaxOnly]
        public JsonResult BestCitizensAjax(IDataTablesRequest request, int? countryID)
        {
            var entityID = SessionHelper.CurrentEntity.EntityID;
            var data = citizenRepository.Query;
            if (countryID.HasValue)
                data = data.Where(c => c.Region.CountryID == countryID);

            var dataCount = data.Count();

            if (string.IsNullOrWhiteSpace(request.Search.Value) == false)
            {
                var query = request.Search.Value.Trim().ToLower();
                data = data.Where(e => e.Entity.Name.ToLower().Contains(query));
            }

            var dataFilteredCount = data.Count();

            if (request.Columns.Get("name").Sort != null)
                data = data.OrderBy(c => c.Entity.Name, request.Columns.Get("name").Sort);
            else if (request.Columns.Get("exp").Sort != null)
                data = data.OrderBy(c => c.ExperienceLevel, request.Columns.Get("exp").Sort).ThenBy(c => c.Experience, request.Columns.Get("exp").Sort);
            else if (request.Columns.Get("mil").Sort != null)
                data = data.OrderBy(c => c.MilitaryRank, request.Columns.Get("mil").Sort);
            else if (request.Columns.Get("medals").Sort != null)
                data = data.OrderBy(c => c.BattleHeroMedals + c.CongressMedals + c.HardWorkerMedals + c.PresidentMedals
                + c.RessistanceHeroMedals + c.SuperJournalistMedals + c.SuperSoldierMedals + c.WarHeroMedals, request.Columns.Get("medals").Sort);
            else
                data = data.OrderByDescending(c => c.ID);

            data = data.Skip(request.Start).Take(request.Length);

            var dataPage = data.Select(c => new
            {
                id = c.ID,
                imgUrl = c.Entity.ImgUrl == null ? Images.Placeholder : c.Entity.ImgUrl,
                name = c.Entity.Name,
                level = c.ExperienceLevel,
                exp = c.Experience,
                mil = c.MilitaryRank,
                medals = c.BattleHeroMedals + c.CongressMedals + c.HardWorkerMedals + c.PresidentMedals 
                + c.RessistanceHeroMedals + c.SuperJournalistMedals + c.SuperSoldierMedals + c.WarHeroMedals

            }).ToList ();


            // Response creation. To create your response you need to reference your request, to avoid
            // request/response tampering and to ensure response will be correctly created.
            var response = DataTablesResponse.Create(request, dataCount, dataFilteredCount, dataPage);

            // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
            // response to a json-compatible content, so DataTables can read it when received.
            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);

        }
    }
}