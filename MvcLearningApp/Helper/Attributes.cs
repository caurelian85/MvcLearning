using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcLearningApp.Entities;

namespace MvcLearningApp.Helper
{
    /// <summary>
    /// Test if session is available
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class CheckSessionValidAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated && filterContext.HttpContext.Session["Id_User"] == null)
            {
                if (filterContext.HttpContext.Request.Cookies["UserInfo"] != null)
                {
                    //load session data
                    var cookie = filterContext.HttpContext.Request.Cookies["UserInfo"];
                    int id;
                    if (int.TryParse(cookie.Value, out id))
                    {
                        //recover user id from cookie
                        filterContext.HttpContext.Session["Id_User"] = id;
                        using (var db = new ForTestingDbEntities())
                        {
                            //check if is admin
                            var name = (from x in db.Users where x.Id == id select x.UserName).First();
                            filterContext.HttpContext.Session["IsAdmin"] = name == "admin";
                        }
                    }
                }
                else
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new JsonResult() { Data = new { Success = false, Message = "auth" } };
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary
                                        {
                                            { "controller", "Account" },
                                            { "action", "LogOn" },
                                            { "ReturnUrl", filterContext.HttpContext.Request.RawUrl }
                                        });
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}