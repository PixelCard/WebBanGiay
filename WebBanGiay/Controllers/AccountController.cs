using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using WebBanGiay.Models;

namespace WebBanGiay.Controllers
{
    public class AccountController : Controller
    {
        private CNPM_LTEntities _dbContext = new CNPM_LTEntities();

        // GET: Account
        public ActionResult LoginPage()
        {
            return View();
        }

        // POST: Account/LoginPage
        [HttpPost]
        public ActionResult LoginPage(string email, string pass)
        {
            var account = _dbContext.Accounts.FirstOrDefault(a => a.Email == email && a.PasswordHash == pass);

            if (account != null)
            {
                // Lưu thông tin vào Session
                Session["Email"] = account.Email;
                Session["Role"] = account.Role.TenRole;

                // Phân quyền chuyển trang
                if (account.Role.TenRole == "Admin")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string Email, string PasswordHash, string UserName)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                var exist = _dbContext.Accounts.FirstOrDefault(a => a.Email == Email);

                if (exist != null)
                {
                    ViewBag.Error = "Email đã tồn tại!";
                    return View();
                }
                // Tạo account mới
                var acc = new Account
                {
                    Email = Email,
                    PasswordHash = PasswordHash, // Nên hash password thực tế
                    UserName = UserName,
                    IDRole = 1 // Mặc định là Customer
                };
                _dbContext.Accounts.Add(acc);
                _dbContext.SaveChanges();
                // Đăng ký thành công, chuyển về LoginPage
                return RedirectToAction("LoginPage", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đăng ký thất bại: " + ex.Message;
                return View();
            }
        }
    }
}