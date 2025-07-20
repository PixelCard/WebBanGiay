using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBanGiay.Filters
{
    public class EncodingFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
            filterContext.HttpContext.Response.HeaderEncoding = System.Text.Encoding.UTF8;
            base.OnActionExecuting(filterContext);
        }
    }
}