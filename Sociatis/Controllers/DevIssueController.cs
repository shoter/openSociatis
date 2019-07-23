using Common.DataTables;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using Entities.enums;
using Entities.Repository;
using Sociatis.ActionFilters;
using Sociatis.Helpers;
using Sociatis.Models.DevIssues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;
using WebServices.structs.Issues;
using WebUtils.Attributes;

namespace Sociatis.Controllers
{
    public class DevIssueController : ControllerBase
    {
        private readonly IDevIssueService devIssueService;
        private readonly IDevIssueRepository devIssueRepository;
        public DevIssueController(IPopupService popupService, IDevIssueService devIssueService, IDevIssueRepository devIssueRepository) : base(popupService)
        {
            this.devIssueService = devIssueService;
            this.devIssueRepository = devIssueRepository;
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("DevIssue/Create")]
        [HttpGet]
        public ActionResult CreateIssue()
        {
            var vm = new CreateIssueViewModel();

            return View(vm);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("DevIssue/Create")]
        [HttpPost]
        public ActionResult CreateIssue(CreateIssueViewModel vm, List<HttpPostedFileBase> files)
        {
            var citizen = SessionHelper.LoggedCitizen;

            var args = new CreateIssueArgs()
            {
                Content = vm.Content,
                Name = vm.Name,
                VisibilityOption = VisibilityOptionEnum.Moderators,
                UploadedFiles = files.ToList()
            };

            var result = devIssueService.CanCreateNewIssue(citizen, args);
            if (result.IsError)
            {
                AddError(result);
                return View(vm);
            }

            var issue = devIssueService.CreateNewIssue(citizen, args);
            AddSuccess("Issue created!");

            return RedirectToAction(nameof(this.ViewIssue), new { issueID = issue.ID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("DevIssue/{issueID:int}/")]
        public ActionResult ViewIssue(int issueID)
        {
            var issue = devIssueRepository.GetById(issueID);
            var citizen = SessionHelper.LoggedCitizen;

            var result = devIssueService.CanSeeIssue(issue, citizen);

            if (result.IsError)
                return RedirectBackWithError(result);

            var vm = new DevIssueViewModel(issue);

            return View(vm);
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("DevIssue/{issueID:int}/{fileID:int}/Download")]
        public void DownloadFile(int issueID, long fileID)
        {
            var issue = devIssueRepository.GetById(issueID);
            var citizen = SessionHelper.LoggedCitizen;

            var result = devIssueService.CanSeeIssue(issue, citizen);
            if (result.IsError)
                return;

            var file = issue.DevIssueUploadedFiles.FirstOrDefault(f => f.ID == fileID);
            if (file == null)
                return;

            TransmitFile(devIssueService.GetPathToUploadedFile(file), file.Filename);
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult SaveModOptions(int issueID, List<DevIssueLabelTypeEnum> labelIDs, VisibilityOptionEnum visibility)
        {
            var issue = devIssueRepository.GetById(issueID);
            var citizen = SessionHelper.LoggedCitizen;

            var result = devIssueService.CanModerateIssue(citizen);
            if (result.IsError)
                return RedirectBackWithError(result);

            devIssueService.SetVisibility(issue, visibility);
            devIssueService.SetLabels(issue, labelIDs.ToArray());

            AddSuccess("Success!");
            return RedirectBack();
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult WriteComment(int issueID, string message, VisibilityOptionEnum visibility = VisibilityOptionEnum.Everyone)
        {
            var issue = devIssueRepository.GetById(issueID);
            var citizen = SessionHelper.LoggedCitizen;

            var result = devIssueService.CanWriteComment(issue, citizen);
            if (result.IsError)
                return RedirectBackWithError(result);

            devIssueService.WriteComment(issue, citizen, message, visibility);
            AddSuccess("Comment added!");
            return RedirectBack();
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Vote(int issueID, int score = 1)
        {
            var issue = devIssueRepository.GetById(issueID);
            var citizen = SessionHelper.LoggedCitizen;

            var result = devIssueService.CanVote(issue, citizen, score);
            if (result.IsError)
                return RedirectBackWithError(result);
            devIssueService.Vote(issue, citizen, score);
            AddSuccess("Voted successfully!");
            return RedirectToAction(nameof(this.ViewIssue), new { issueID = issueID });
        }


        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [HttpPost]
        public ActionResult Unvote(int issueID)
        {
            var issue = devIssueRepository.GetById(issueID);
            var citizen = SessionHelper.LoggedCitizen;

            var result = devIssueService.CanUnvote(issue, citizen);
            if (result.IsError)
                return RedirectBackWithError(result);
            devIssueService.Unvote(issue, citizen);
            AddSuccess("Voted successfully!");
            return RedirectToAction(nameof(this.ViewIssue), new { issueID = issueID });
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("DevIssue/")]
        public ActionResult Index()
        {
            return View();
        }

        [SociatisAuthorize(PlayerTypeEnum.Player)]
        [Route("DevIssue/IssuesAjax")]
        [HttpPost]
        [AjaxOnly]
        public JsonResult IssuesAjax(IDataTablesRequest request, bool? onlyUnresolved)
        {
            var citizen = SessionHelper.LoggedCitizen;
            var visibility = devIssueService.GetVisiblityOption(citizen);

            var data = devIssueRepository.Query
                .Where(v => v.VisibilityOptionID <= (int)visibility
                ||
                (v.VisibilityOptionID == (int)VisibilityOptionEnum.Author && v.CreatedByID == citizen.ID));
            var dataCount = data.Count();

            if (onlyUnresolved == true)
                data = data.Where(i => i.DevIssueLabelTypes.Any(l => l.ID == (int)DevIssueLabelTypeEnum.Resolved) == false);

            var dataFilteredCount = data.Count();



            if (request.Columns.Get("votes").Sort != null)
                data = data.OrderBy(c => c.DevIssueVotes.Count, request.Columns.Get("votes").Sort);
            else if (request.Columns.Get("day").Sort != null)
                data = data.OrderBy(c => c.Day, request.Columns.Get("day").Sort);
            else if (request.Columns.Get("lastActivity").Sort != null)
                data = data.OrderBy(i => i.Day >= (i.DevIssueComments.Max(x => (int?)x.Day) ?? 0) ? i.Day : (i.DevIssueComments.Max(x => (int?)x.Day) ?? 0), request.Columns.Get("lastActivity").Sort);
            else
                data = data
                .OrderByDescending(i => i.DevIssueVotes.Sum(v => v.Score));

            data = data.Skip(request.Start).Take(request.Length);


            var dataPage = data.Select(i => new
            {
                ID = i.ID,
                CreatorName = i.Citizen.Entity.Name,
                CreatorID = i.CreatedByID,
                Name = i.Name,
                Day = i.Day,
                LastActivity = i.Day >= (i.DevIssueComments.Max(x => (int?)x.Day) ?? 0) ? i.Day : (i.DevIssueComments.Max(x => (int?)x.Day) ?? 0),
                Labels = i.DevIssueLabelTypes.Select(l => l.ID),
                Visibility = i.VisibilityOption.Name,
                Votes = i.DevIssueVotes.Sum(v => (int?)v.Score) ?? 0
            }).ToList()
            .Select(d => new
            {
                data = d,
                Labels = d.Labels.Select(l => new
                {
                    Name = ((DevIssueLabelTypeEnum)l).ToString(),
                    Classname = DevIssueLabelViewModel.GetClassname((DevIssueLabelTypeEnum)l)
                })
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