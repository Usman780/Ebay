using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EbayProject.Models;
using EbayProject.BL;
using EbayProject.Helping_Classes;
using System.Security.Claims;
using System.Threading;

namespace EbayProject.Controllers
{
    [ValidationFilter(CheckLogin = false)]
    public class AuthController : Controller
    {
        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly DatabaseEntities de = new DatabaseEntities();

        public ActionResult Login(string msg = "", string color = "black")
        {
            if (gp.ValidateLoggedinUser() != null)
            {
                return RedirectToAction("Index", "Admin");
            }

            int userCount = new UserBL().GetActiveUsersList(de).Count();
            if (userCount == 0)
            {
                User obj = new User()
                {
                    Name = "Usman Ali",
                    Contact = "0000-0000000",
                    Email = "usman78056@gmail.com",
                    Password = StringCipher.Encrypt("123"),
                    Role = 1,
                    IsActive = 1,
                    CreatedAt = DateTime.Now
                };

                bool chkUser = new UserBL().AddUser(obj, de);
            }

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult PostLogin(string Email = "", string Password = "")
        {
            User user = new UserBL().GetActiveUsersList(de).Where(x => x.Email.Trim().ToLower() == Email.Trim().ToLower() && StringCipher.Decrypt(x.Password).Equals(Password)).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Login", new { msg = "Incorrect Email/Password!", color = "red" });
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid,user.Id.ToString()),
                new Claim("UserName", user.Name),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Role", user.Role.ToString()),

            }, "ApplicationCookie");

            var claimsPrincipal = new ClaimsPrincipal(identity); // Set current principal
            Thread.CurrentPrincipal = claimsPrincipal;
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignIn(identity);

            return RedirectToAction("Index", "Admin");
        }


        #region Signup

        //public ActionResult Register(string msg = "", string color = "black")
        //{
        //    ViewBag.Message = msg;
        //    ViewBag.Color = color;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult PostRegister(User _user, string _confirmPassword = "")
        //{
        //    if (_user.Password != _confirmPassword)
        //    {
        //        return RedirectToAction("Register", "Auth", new { msg = "Password and confirm password didn't match", color = "red" });
        //    }

        //    bool checkEmail = gp.ValidateEmail(_user.Email);
        //    if (checkEmail == false)
        //    {
        //        return RedirectToAction("Register", "Auth", new { msg = "Email already exists. Try sign in!", color = "red" });
        //    }

        //    User u = new User()
        //    {
        //        Name = _user.Name.Trim(),
        //        Contact = _user.Contact,
        //        Email = _user.Email.Trim(),
        //        Password = StringCipher.Encrypt(_user.Password),
        //        Role = 3,
        //        IsActive = 1,
        //        CreatedAt = DateTime.Now
        //    };

        //    bool chkUser = new UserBL().AddUser(u, de);

        //    if (chkUser)
        //    {
        //        return RedirectToAction("Login", "Auth", new { msg = "Account created successfully, Please login", color = "green" });
        //    }
        //    else
        //    {
        //        return RedirectToAction("Register", "Auth", new { msg = "Somethings' wrong", color = "red" });
        //    }
        //}

        #endregion


        #region Forgot Password

        public ActionResult ForgotPassword(string msg = "", string color = "black")
        {
            ViewBag.Color = color;
            ViewBag.Message = msg;

            return View();
        }

        public ActionResult PostForgotPassword(string Email = "")
        {
            bool checkEmail = gp.ValidateEmail(Email);

            if (checkEmail == false)
            {
                string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");

                bool checkMail = MailSender.SendForgotPasswordEmail(Email, BaseUrl);

                if (checkMail == true)
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Please check your inbox/spam", color = "green" });
                }
                else
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Mail sending fail!", color = "red" });
                }
            }
            else
            {
                return RedirectToAction("ForgotPassword", "Auth", new { msg = "Email does not belong to our record!!", color = "red" });
            }

        }


        public ActionResult ResetPassword(string email = "", string time = "", string msg = "", string color = "black")
        {
            DateTime PassDate = Convert.ToDateTime(StringCipher.Base64Decode(time)).Date;
            DateTime CurrentDate = DateTime.Now.Date;

            if (CurrentDate != PassDate)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Link expired, Please try again!", color = "red" });
            }


            ViewBag.Time = time;
            ViewBag.Email = email;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult PostResetPassword(string Email = "", string Time = "", string NewPassword = "", string ConfirmPassword = "")
        {
            if (NewPassword != ConfirmPassword)
            {
                return RedirectToAction("ResetPassword", "Auth", new { email = Email, time = Time, msg = "Password and confirm password did not match", color = "red" });
            }

            string DecryptEmail = StringCipher.Base64Decode(Email);

            DatabaseEntities de = new DatabaseEntities();
            User user = new UserBL().GetActiveUsersList(de).Where(x => x.Email.Trim().ToLower() == DecryptEmail.Trim().ToLower()).FirstOrDefault();

            user.Password = StringCipher.Encrypt(NewPassword);

            bool check = new UserBL().UpdateUser(user, de);

            if (check == true)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Password reset successful, Try login", color = "green" });
            }
            else
            {
                return RedirectToAction("ResetPassword", "Auth", new { email = Email, time = Time, msg = "Somethings' wrong!", color = "red" });
            }
        }

        #endregion


        #region Manage User Profile

        [ValidationFilter(CheckLogin = true)]
        public ActionResult UpdateProfile(string msg = "", string color = "black")
        {
            User u = new UserBL().GetActiveUserById(gp.ValidateLoggedinUser().Id, de);

            ViewBag.User = u;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


        [ValidationFilter(CheckLogin = true)]
        public ActionResult PostUpdateProfile(string EncId, User _user)
        {
            int UserId = Convert.ToInt32(StringCipher.Decrypt(EncId));

            bool checkEmail = gp.ValidateEmail(_user.Email, UserId);

            if (checkEmail == false)
            {
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Email used by someone else, Please try another", color = "red" });
            }

            DatabaseEntities de = new DatabaseEntities();
            User u = new UserBL().GetActiveUserById(UserId, de);

            u.Name = _user.Name.Trim();
            u.Contact = _user.Contact.Trim();
            u.Email = _user.Email.Trim();

            bool chk = new UserBL().UpdateUser(u, de);

            if (chk == true)
            {
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Profile updated successfully!", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Somthings' Wrong!", color = "red" });
            }

        }



        [ValidationFilter(CheckLogin = true)]
        public ActionResult UpdatePassword(string msg = "", string color = "black")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


        [ValidationFilter(CheckLogin = true)]
        public ActionResult PostUpdatePassword(string oldPassword = "", string newPassword = "", string confirmPassword = "")
        {
            if (newPassword != confirmPassword)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "New password and Confirm password did not match!", color = "red" });
            }

            DatabaseEntities de = new DatabaseEntities();
            User u = new UserBL().GetActiveUserById(gp.ValidateLoggedinUser().Id, de);

            if (StringCipher.Decrypt(u.Password) != oldPassword)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Old password did not match!", color = "red" });
            }

            u.Password = StringCipher.Encrypt(newPassword);

            bool chk = new UserBL().UpdateUser(u, de);

            if (chk == true)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Password updated successfully!", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Somthings' wrong!", color = "red" });
            }
        }

        #endregion


        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login");
        }
    }
}