using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Models.Searchs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    public class SearchController : ControllerBase
    {
        private readonly IEntityRepository entityRepository;

        public SearchController(IPopupService popupService, IEntityRepository entityRepository) : base(popupService)
        {
            this.entityRepository = entityRepository;
        }

        [SociatisAuthorize(Entities.enums.PlayerTypeEnum.Player)]
        [Route("Search/{query}")]
        public ActionResult Search(string query)
        {
            query = query.Trim().ToLower();
            var entities = entityRepository.Where(e => e.Name.ToLower().Contains(query))
                .OrderBy(e => e.EntityID).Take(20).ToList();

            var vm = new SearchListViewModel(entities);

            return View(vm);
        }

        [SociatisAuthorize(Entities.enums.PlayerTypeEnum.Player)]
       [HttpPost]
        public ActionResult PostSearch(string query)
        {
            return RedirectToAction("Search", new { query = query });
        }
    }
}