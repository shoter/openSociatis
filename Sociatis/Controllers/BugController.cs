using Entities;
using Entities.Repository;
using Sociatis.Code.Filters;
using Sociatis.Helpers;
using Sociatis.Models.Bugs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServices;

namespace Sociatis.Controllers
{
    [DayChangeAuthorize]
    public class BugController : ControllerBase
    {
        private readonly IBugReportRepository bugReportRepository;
        private readonly IUploadService uploadService;

        public BugController(IBugReportRepository bugReportRepository, IUploadService uploadService, IPopupService popupService) : base(popupService)
        {
            this.bugReportRepository = bugReportRepository;
            this.uploadService = uploadService;
        }

        [HttpGet]
        public ActionResult ReportBug()
        {
            return View(new BugReportViewModel());
        }

        [HttpPost]
        public ActionResult ReportBug(BugReportViewModel vm)
        {
            string output = "";
            if(ModelState.IsValid)
            {
                try
                {
                    vm.Content = vm.Content.Replace(Environment.NewLine, "<br/>");
                    string filename = null;


                    if (vm.BugImage != null)
                    {
                        string path = uploadService.UploadImage(vm.BugImage, WebServices.enums.UploadLocationEnum.Bugs, enableSizeValidation: false);
                        filename = Path.GetFileName(path);
                    }



                    var entity = new BugReport()
                    {
                        Content = vm.Content,
                        CitizenID = SessionHelper.LoggedCitizen?.ID,
                        Citizen = SessionHelper.LoggedCitizen,
                        ImgUrl = filename == null ? null : "upload/" + filename
                    };

                    bugReportRepository.Add(entity);
                    bugReportRepository.SaveChanges();
                    

                    vm.Sent = true;
                } 
                catch(Exception e)
                {
                    return Content(output + "There was a problem with your bug report. Try to go back and save details about bug. Inform admin about this bug by messaging account 'admin'. Try to send bug later. <br/><br/>We are doomed if there are bug errors in bug reporting service :O."
                        + e.ToString().Replace(Environment.NewLine, "<br/>")
                        );
                }
            }
            return View(vm);
        }
    }
}