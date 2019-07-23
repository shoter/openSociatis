using Common.Exceptions;
using Common.Operations;
using Common.Transactions;
using Entities.Repository;
using Sociatis.Code.Json;
using Sociatis.Helpers;
using Sociatis.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebServices;
using WebServices.enums;
using WebServices.Models;
using WebUtils.Forms.Select2;

namespace Sociatis.Controllers
{
    public class ControllerBase : Controller
    {
        private readonly IPopupService popupService;
        private readonly ISessionRepository sessionRepository;
        protected readonly ITransactionScopeProvider transactionScopeProvider;

        public ControllerBase(IPopupService popupService)
        {
            this.popupService = popupService;
            this.sessionRepository = DependencyResolver.Current.GetService<ISessionRepository>();
            this.transactionScopeProvider = DependencyResolver.Current.GetService<ITransactionScopeProvider>();
        }
        /// <summary>
        /// Add message which will be displayed after next HTTP Request
        /// You can add multiple messages which will be stacked on the page
        /// </summary>
        /// <param name="message">message to display.</param>
        protected void AddMessage(PopupMessageViewModel message)
        {
            popupService.AddMessage(message);
        }

        public JsonResult UndefinedJsonError(Exception e)
        {
#if DEBUG
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(e));
            return JsonError(e.Message);
#else
            Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Elmah.Error(e));
            return JsonError("Undefined error");

#endif
        }

        public new JsonResult Response(IQueryable<ISelect2Item> query, ISelect2Request request)
        {
            return Json(new Select2Response(query, request), JsonRequestBehavior.AllowGet);
        }

        protected void AddMessage(string content, PopupMessageType type)
        {
            AddMessage(new PopupMessageViewModel(content, type));
        }

        protected void AddError(string content)
        {
            AddMessage(content, PopupMessageType.Error);
        }

        protected void AddError(IValidator validator)
        {
            foreach (var error in validator.ValidationErrors)
            {
                AddError(error.Error);
            }
        }

        protected void AddError(string content, params object[] args)
        {
            content = string.Format(content, args);
            AddError(content);
        }

        protected void AddError(MethodResult result)
        {
            foreach (var error in result.Errors)
            {
                AddError(error);
            }
        }

        protected void AddWarning(string content)
        {
            AddMessage(content, PopupMessageType.Warning);
        }

        protected void AddInfo(string content)
        {
            AddMessage(content, PopupMessageType.Info);
        }

        protected void AddSuccess(string content)
        {
            AddMessage(content, PopupMessageType.Success);
        }
        protected void AddSuccess(string content, params object[] args)
        {
            AddMessage(string.Format(content, args), PopupMessageType.Success);
        }
        protected void AddInfo(string content, params object[] args)
        {
            content = string.Format(content, args);
            AddInfo(content);
        }


        public HttpStatusCodeResult ServerError(string message)
        {
            return new HttpStatusCodeResult(500, message);
        }

        public RedirectToRouteResult RedirectToHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RedirectToHomeWithError(string errorMessage = "Error")
        {
            AddError(errorMessage);
            return RedirectToHome();
        }

        public ActionResult RedirectToHomeWithError(MethodResult result)
        {
            AddError(result);
            return RedirectToHome();
        }

        public ActionResult RedirectBack()
        {
            if (Request.UrlReferrer == null)
                return RedirectToHome();

            return Redirect(Request.UrlReferrer.ToString());
        }
        public ActionResult RedirectBackWithError(string errorMessage = "Error")
        {
            AddError(errorMessage);
            return RedirectBack();
        }

        public ActionResult RedirectBackWithError(MethodResult error)
        {
            AddError(error);
            return RedirectBack();
        }

        public ActionResult RedirectBackWithInfo(string infoMessage)
        {
            AddInfo(infoMessage);
            return RedirectBack();
        }

        public ActionResult RedirectBackWithSuccess(string message)
        {
            AddSuccess(message);
            return RedirectBack();
        }

        protected virtual JsonResult JsonDebugOnlyError(Exception e)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(e);
#if DEBUG
            return JsonError(e.Message);
#else
            if (e is UserReadableException)
                return JsonError(e.Message);

            return JsonError("Error");
#endif
        }

        public virtual JsonResult Select2Response(IQueryable<ISelect2Item> query, ISelect2Request request)
        {
            return Json(
                new Select2Response(query, request),
                JsonRequestBehavior.AllowGet);
        }

        protected virtual JsonResult JsonSelectList(List<SelectListItem> selectList)
        {
            return Json(selectList);
        }
        protected virtual JsonResult JsonError(string errorMessage)
        {
            return Json(new JsonErrorModel(errorMessage));
        }

        protected virtual JsonResult JsonRedirect(string actionName)
        {
            return Json(new JsonRedirectModel(Url.Action(actionName)));
        }

        protected virtual JsonResult JsonRedirect(string actionName, string controllerName)
        {
            return Json(new JsonRedirectModel(Url.Action(actionName, controllerName, null, Request.Url.Scheme)));
        }

        protected virtual JsonResult JsonRedirect(string actionName, string controllerName, object routeValues)
        {
            return Json(new JsonRedirectModel(Url.Action(actionName, controllerName, routeValues, Request.Url.Scheme)));
        }

        protected virtual JsonResult JsonRedirect(string actionName, object routeValues)
        {
            return Json(new JsonRedirectModel(Url.Action(actionName, null, routeValues, Request.Url.Scheme)));
        }

        protected virtual JsonResult JsonError(Exception e)
        {
            return JsonError(e.Message);
        }

        protected virtual JsonResult JsonError(MethodResult result)
        {
            return Json(new JsonErrorModel(result));
        }

        protected virtual JsonResult JsonDebugError(Exception e)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            return Json(new JsonDebugErrorModel(e));
        }

        protected virtual JsonResult JsonSuccess(string message)
        {
            return Json(new JsonSuccessModel(message));
        }

        protected virtual JsonResult JsonData(object data)
        {
            return Json(new JsonDataModel(data));
        }

        protected virtual JsonResult JsonSuccess(string message, params object[] args)
        {
            return JsonSuccess(string.Format(message, args));
        }

        protected virtual JsonResult JsonWarning(string message)
        {
            return Json(new JsonWarningModel(message));
        }

        protected virtual JsonResult JsonPartial(string content)
        {
            return Json(new JsonPartialModel(content));
        }

        public string RenderPartialToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewDataDictionary ViewData = new ViewDataDictionary();
            TempDataDictionary TempData = new TempDataDictionary();
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }


        }

        protected void TransmitFile(string filePath)
        {
            TransmitFile(filePath, Path.GetFileName(filePath));
        }

        protected void TransmitFile(string filePath, string filename)
        {
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition",
                               "attachment; filename=" + filename + ";");
            response.TransmitFile(filePath);
            response.Flush();
            response.End();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction == false)
                foreach (var msg in popupService.Popups)
                    TempData.AddMessage(msg);

            var session = SessionHelper.Session;

            if (session?.ExpirationDate.CompareTo(DateTime.Now) > 0)
            {
                sessionRepository.UpdateSession(session, DateTime.Now.AddHours(session.RememberMe == true ? 7 * 24 : 1));
            }
            else if (session?.ExpirationDate.CompareTo(DateTime.Now) < 0)
            {
                using (var trs = DependencyResolver.Current.GetService<ITransactionScopeProvider>().CreateTransactionScope())
                {
                    if (sessionRepository.TryRemove(session.ID))
                        sessionRepository.SaveChanges();
                    trs.Complete();
                }
            }

            base.OnActionExecuted(filterContext);
        }


    }
}
