﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EbayProject.Helping_Classes
{
    public class ValidationFilter : FilterAttribute, IActionFilter, IExceptionFilter
    {
        public int Role;
        public bool CheckLogin;
        private readonly GeneralPurpose gp = new GeneralPurpose();
        //constructor
        public ValidationFilter()
        {
            CheckLogin = true;
        }


        //exception handling
        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            string action = filterContext.RouteData.Values["action"].ToString();
            Exception e = filterContext.Exception;
            filterContext.ExceptionHandled = true;

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                    { "controller", "Error" },{ "action", "NotFound" }, });
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (CheckLogin == true) //only works when check is true
            {
                if (gp.ValidateLoggedinUser() == null)
                {
                    var values = new RouteValueDictionary(new
                    {
                        action = "Login",
                        controller = "Auth",
                        msg = "Session expired, Please login",
                        color = "red"
                    });

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(values));
                }
                else
                {

                    if (Role != 0)
                    {
                        if (gp.ValidateLoggedinUser().Role != Role)
                        {
                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                                { "controller", "Error" },{ "action", "NotFound" }, });
                        }
                    }
                }

            }

        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

    }
}