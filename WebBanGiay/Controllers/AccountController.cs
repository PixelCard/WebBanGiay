using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebBanGiay.Models;
using WebBanGiay.Filters;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace WebBanGiay.Controllers
{
    [EncodingFilter]
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
                Session["UserID"] = account.IDTK;

                // Phân quyền chuyển trang
                if (account.Role.TenRole == "Admin")
                    return RedirectToAction("Index", "AdminPage", new { area = "Admin" });
                else if (account.Role.TenRole == "Employess") 
                    return RedirectToAction("OrdersByDate", "Statistics", new { area = "NhanVienBanHang" });
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

        public ActionResult Logout()
        {
            // Xóa session
            Session.Clear();
            Session.Abandon();
            
            // Chuyển về trang chủ
            return RedirectToAction("LoginPage", "Account");
        }

        public ActionResult AccountDetails()
        {
            // Kiểm tra đăng nhập
            if (Session["Email"] == null)
            {
                return RedirectToAction("LoginPage");
            }

            // Lấy thông tin tài khoản từ session
            string email = Session["Email"].ToString();
            var account = _dbContext.Accounts.FirstOrDefault(a => a.Email == email);

            if (account == null)
            {
                Session.Clear();
                return RedirectToAction("LoginPage");
            }

            return View(account);
        }

        public ActionResult EditAccount()
        {
            // Kiểm tra đăng nhập
            if (Session["Email"] == null)
            {
                return RedirectToAction("LoginPage");
            }

            // Lấy thông tin tài khoản và customer từ session
            string email = Session["Email"].ToString();
            var account = _dbContext.Accounts.Include("Customers").FirstOrDefault(a => a.Email == email);

            if (account == null)
            {
                Session.Clear();
                return RedirectToAction("LoginPage");
            }

            // Lấy thông tin customer nếu có
            var customer = account.Customers.FirstOrDefault();
            
            ViewBag.Customer = customer;
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(string UserName, string FullName, string Phone, string Address, string Gender, string PreferredSize, string PreferredBrand)
        {
            // Kiểm tra đăng nhập
            if (Session["Email"] == null)
            {
                return RedirectToAction("LoginPage");
            }

            try
            {
                string email = Session["Email"].ToString();
                var account = _dbContext.Accounts.Include("Customers").FirstOrDefault(a => a.Email == email);

                if (account == null)
                {
                    Session.Clear();
                    return RedirectToAction("LoginPage");
                }

                // Cập nhật thông tin tài khoản
                account.UserName = UserName;

                // Cập nhật hoặc tạo thông tin customer
                var customer = account.Customers.FirstOrDefault();
                if (customer == null)
                {
                    // Tạo customer mới
                    customer = new Customer
                    {
                        FullName = FullName,
                        Phone = Phone,
                        Address = Address,
                        Gender = Gender,
                        PreferredSize = PreferredSize,
                        PreferredBrand = PreferredBrand,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        IDAccount = account.IDTK
                    };
                    _dbContext.Customers.Add(customer);
                }
                else
                {
                    // Cập nhật customer hiện có
                    customer.FullName = FullName;
                    customer.Phone = Phone;
                    customer.Address = Address;
                    customer.Gender = Gender;
                    customer.PreferredSize = PreferredSize;
                    customer.PreferredBrand = PreferredBrand;
                }

                _dbContext.SaveChanges();
                ViewBag.Success = "Cập nhật thông tin thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra: " + ex.Message;
            }

            // Load lại dữ liệu để hiển thị
            string emailReload = Session["Email"].ToString();
            var accountReload = _dbContext.Accounts.Include("Customers").FirstOrDefault(a => a.Email == emailReload);
            var customerReload = accountReload?.Customers.FirstOrDefault();
            ViewBag.Customer = customerReload;

            return View(accountReload);
        }
    }
}