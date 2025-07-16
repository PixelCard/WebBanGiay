using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace WebBanGiay.Models.model_class
{
    public class Account
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int IDRole { get; set; } // Khóa ngoại đến bảng Role
        public virtual Role Role { get; set; } 
    }
}