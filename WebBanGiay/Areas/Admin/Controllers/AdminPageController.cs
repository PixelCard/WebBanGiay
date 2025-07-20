using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Filters;

namespace WebBanGiay.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class AdminPageController : Controller
    {
        // GET: Admin/AdminPage
        public ActionResult Index()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("LoginPage", "Account", new { area = "" });
            }

            return View();
        }
    }
}