using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using Entities.enums;
using Entities.Models.Finances;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.CompanyFinances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;
using WebServices.Helpers;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    public class CompanyFinanceController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly ICompanyFinanceSummaryRepository companyFinanceSummaryRepository;
        private readonly ICompanyService companyService;
        private readonly ICompanyFinanceService companyFinanceService;

        public CompanyFinanceController(IPopupService popupService, ICompanyRepository companyRepository,
            ICompanyFinanceSummaryRepository companyFinanceSummaryRepository, ICompanyService companyService,
            ICompanyFinanceService companyFinanceService) : base(popupService)
        {
            this.companyRepository = companyRepository;
            this.companyFinanceSummaryRepository = companyFinanceSummaryRepository;
            this.companyService = companyService;
            this.companyFinanceService = companyFinanceService;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}/Finances/day-{day:int}/Summary")]
        [Route("Company/{companyID:int}/Finances/Summary")]
        public ActionResult DaySummary(int companyID, int? day)
        {
            day = day ?? GameHelper.CurrentDay;
            var company = companyRepository.GetById(companyID);
            var result = companyFinanceService.CanAccessFinances(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            if (result.IsError)
                return RedirectBackWithError(result);

            var summaries = companyFinanceSummaryRepository.GetSummariesForDay(companyID, day.Value);

            var vm = new DaySummaryViewModel(company, summaries, day.Value);

            return View(vm);
        }
        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("Company/{companyID:int}/Finances/Overview")]
        public ActionResult Overview(int companyID)
        {
            var company = companyRepository.GetById(companyID);
            var result = companyFinanceService.CanAccessFinances(company, SessionHelper.CurrentEntity, SessionHelper.LoggedCitizen);

            if (result.IsError)
                return RedirectBackWithError(result);

            var vm = new FinanceOverviewViewModel(company);

            return View(vm);
        }


        [HttpPost]
        [AjaxOnly]
        [Route("Company/{companyID:int}/Finances/OverviewAjax")]
        public ActionResult OverviewAjax(IDataTablesRequest request, int companyID, int? currencyID)
        {
            var data = companyFinanceSummaryRepository.Where(s => s.CompanyID == companyID);
            if (currencyID.HasValue)
                data = data.Where(s => s.CurrencyID == currencyID);
            var dataCount = data.Count();

            var dataFilteredCount = data.Count();

            if (request.Columns.First()?.Sort?.Direction == SortDirection.Ascending)
                data = data.OrderBy(o => o.Day);
            else
                data = data.OrderByDescending(o => o.Day);


            data = data.Skip(request.Start).Take(request.Length);

            var dataPage = data.ToList().Select(s => new
            {
                day = s.Day,
                currencySymbol = s.Currency.Symbol,
                total = new CompanyFinance(s).Total
            });

            // Response creation. To create your response you need to reference your request, to avoid
            // request/response tampering and to ensure response will be correctly created.
            var response = DataTablesResponse.Create(request, dataCount, dataFilteredCount, dataPage);

            // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
            // response to a json-compatible content, so DataTables can read it when received.
            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);

        }
    }
}
