using System;
using System.Web;
using System.Web.Mvc;

namespace WebBanGiay.Filters
{
    public class NhanVienBanHangAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var session = httpContext.Session;
            if (session == null || session["Role"] == null || session["Role"].ToString() != "Employess")
            {
                return false;
            }
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirect về trang đăng nhập nếu không hợp lệ
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary(
                    new
                    {
                        controller = "Account",
                        action = "LoginPage",
                        area = ""
                    })
            );
        }
    }
} 