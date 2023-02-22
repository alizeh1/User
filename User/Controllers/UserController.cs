using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using User.Models;
using System.Configuration;
using System.Web.Security;
using System.Data;

namespace User.Controllers
{

    public class UserController : Controller
    {
        BALUser objBalUser = new BALUser();


        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult Login()
        {

            return PartialView("Login");

        }

        [HttpPost]
        public ActionResult Login(SignIn UserSignIn)
        {
           //Session["UserName"] = UserSignIn.Email;
            string passWord = encryptPassword.textToEncrypt(UserSignIn.Password);
            SqlDataReader drSignIn;
            drSignIn = objBalUser.SignIn(UserSignIn.Email, passWord);
            while (drSignIn.Read()) {
                string email = drSignIn["Email"].ToString();
                string password = drSignIn["Password"].ToString();
                bool emailverification = (bool)drSignIn["EmailVerification"];
                if (email == UserSignIn.Email && password == passWord && emailverification == true)
                {
                    int timeout = UserSignIn.Rememberme ? 60 : 1; // Timeout in minutes, 60 = 1 hour.    
                    //var ticket = new FormsAuthenticationTicket(UserSignIn.Email, false, timeout);
                    //string encrypted = FormsAuthentication.Encrypt(ticket);
                    //var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    //Response.Cookies.Add(cookie1);
                   
                    //cookie.HttpOnly = true;
                    HttpCookie cookie = new HttpCookie("name");
                    cookie.Value = UserSignIn.Email;
                    cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                    HttpContext.Response.Cookies.Add(cookie);  //save in browser
                    // FormsAuthentication.GetAuthCookie(UserSignIn.Email);
                    TempData["MessageForLogin"] = "<script>alert('Login Successfully..!!')</script>";
                    return RedirectToAction("Alisha");
                }
               
            }
                ViewBag.MessageForInvalidLogin = "Login Failed ?";
            
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Alisha()
        {
            HttpCookie cookie = Request.Cookies["name"];
            if (cookie!=null)
            {
                ViewBag.Name = Request.Cookies["name"].Value.ToString();
            }
            else
            {
                return RedirectToAction("Index");
            }
            //if (Session["UserName"] == null)
            //{
            //    return RedirectToAction("Index");
            //}
            return View();
        }


        [Authorize]
        public ActionResult LogOut()
        {
            //Session.Abandon();           //destroy session
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "User");
        }

       

        [HttpGet]
        public ActionResult SignUp()
        {

            return View();
        }

        public bool IsEmailExists(string eMail)
        {
            SqlDataReader drEmail;
            drEmail = objBalUser.EmailVerification(eMail);

            while (drEmail.Read())
            {
                string Email = drEmail["Email"].ToString();

                if (Email == eMail)
                {
                    ViewBag.email = "";

                }
            }
            return ViewBag.email != null;
        }

        public void SendEmailToUser(string emailId, string activationCode,String password)
        {
            var GenarateUserVerificationLink = "/User/UserVerification/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            var fromMail = new MailAddress("alishamaljapte62@gmail.com"); // set your email    
            var fromEmailpassword = "eojcfmozssuaazmg"; // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Registration Completed";
            Message.Body = "<br/> Your registration completed succesfully." +
                           "<br/> please click on the below link for account verification" +
                           "<br/> Password is :"+ password +
                           "<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }

        [HttpPost]
        public ActionResult SignUp(UserRegistration UserReg)
        {
            // email not verified on registration time    
            UserReg.EmailVerification = false;
            //Email Verification
            var IsExits = IsEmailExists(UserReg.Email);
            if (IsExits)
            {
                ModelState.AddModelError("EmailExits", "Email Already Exits");
                return View("SignUp");
            }
            //it generate unique code
            UserReg.ActivetionCode = Guid.NewGuid().ToString();
            //password convert    
            //UserReg.Password =
            string password= encryptPassword.textToEncrypt(UserReg.Password);
            objBalUser.RegisterUser(UserReg.FirstName, UserReg.LastName, UserReg.Email, password, UserReg.EmailVerification, UserReg.ActivetionCode);
            //send email verification link to user
            SendEmailToUser(UserReg.Email, UserReg.ActivetionCode,UserReg.Password);
            var Message = "Registration Completed.Please check your Email: " + UserReg.Email;
            ViewBag.Message = Message;
            return View("Registration");
        }

        // Verification from Email Account.    
        public ActionResult UserVerification(string id)
        {
            //bool Status = false;

            SqlDataReader drActivationCode;
            drActivationCode = objBalUser.UserVerification(id);
            try {
                while (drActivationCode.Read())
                {
                    string ActivetionCode = drActivationCode["ActivetionCode"].ToString();

                    if (ActivetionCode == id)
                    {
                        bool Status = true;
                        objBalUser.UpdateEmailVerificationStatus(Status, id);
                        // ViewBag.Message = "Email Verification completed";



                    }
                    else
                    {
                        ViewBag.Message = "Invalid Request...Email not verify";
                        //ViewBag.Status = false;
                    }


                }
            }
            catch (InvalidOperationException) {

                ViewBag.Message = "Email Verification completed";
            }



            return View();
        }
       
       

        public ActionResult Registration()
        {
            return View();
        }


        public void ForgetPasswordEmailToUser(string emailId, string activetioncode, string otp)
        {
            
            var GenarateUserVerificationLink = "/User/ChangePassword/"+ activetioncode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            var fromMail = new MailAddress("alishamaljapte62@gmail.com"); // set your email    
            var fromEmailpassword = "eojcfmozssuaazmg"; // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Password Reset";
            Message.Body =
                           "<br/> please click on the below link for password change" +
                           "<br/><br/><a href=" + link + ">" + link + "</a>" +
                            "<br/> OTP for password change:" + otp;
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }

        public string GeneratePassword()
        {
            string OTPLength = "4";
            string OTP = string.Empty;

            string Chars = string.Empty;
            Chars = "1,2,3,4,5,6,7,8,9,0";

            char[] seplitChar = { ',' };
            string[] arr = Chars.Split(seplitChar);
            string NewOTP = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                NewOTP += temp;
                OTP = NewOTP;
            }
            return OTP;
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(ForgetPassword forgetP)
        {
            var IsExits = IsEmailExists(forgetP.EmailId);
            if (!IsExits)
            {
                ModelState.AddModelError("EmailNotExists", "This email is not exists");
                return View();
            }
            UserRegistration objUserReg = new UserRegistration();
            string otp = GeneratePassword();
            objUserReg.ActivetionCode = Guid.NewGuid().ToString();
            forgetP.OTP = otp;
            objBalUser.ForgetPassword(forgetP.EmailId, objUserReg.ActivetionCode.ToString(), forgetP.OTP);
            ForgetPasswordEmailToUser(forgetP.EmailId, objUserReg.ActivetionCode, forgetP.OTP);
            ViewBag.ForgetPassword = $"Please check your mail :{forgetP.EmailId}";


            return View();
        }

        public void ChangePasswordEmailToUser(string emailId, string password)
        {
            var GenarateUserVerificationLink = "/User/Index/";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            var fromMail = new MailAddress("alishamaljapte62@gmail.com"); // set your email    
            var fromEmailpassword = "eojcfmozssuaazmg"; // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Password Changed";
            Message.Body =
                           "<br/>Hey, " +
                           "<br/>Your new password is below Please check" +
                            "<br/>Mail Id is " + emailId +
                            "<br/>Password is " + password +
                            "<br/>Click below link for signIn" +
                            "<br/><br/><a href=" + link + ">" + link + "</a>";

            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }


        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string id, ChangePassword objChangeP)
        {
            
            SqlDataReader drEmailForChangeP;
            
            drEmailForChangeP = objBalUser.GetEmailForchangePassword(id);
            try
            {
                while (drEmailForChangeP.Read())
                {
                    string email = drEmailForChangeP["Email"].ToString();
                    string otp = drEmailForChangeP["OTP"].ToString();

                    if (otp == objChangeP.OTP)
                    {
                        string password = encryptPassword.textToEncrypt(objChangeP.Password);
                        objBalUser.ChangePassword(email, password);
                        ChangePasswordEmailToUser(email, objChangeP.Password);
                        ViewBag.MessageSuccess = "Password Changed successfully..Please Check your mail";
                        // return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.Message = "Enter correct OTP";
                    }
                }
            }
            catch (InvalidOperationException)
            {

            }
           
            
           
            return View();
        }


        /*****************************************Save User Details*****************************************/


        public void Country_Bind()
        {
            DataSet dsCountry=new DataSet();
            dsCountry = objBalUser.Get_Country();
            List<SelectListItem> coutrylist = new List<SelectListItem>();
            foreach (DataRow dr in dsCountry.Tables[0].Rows)
            {

                coutrylist.Add(new SelectListItem { Text = dr["CountryName"].ToString(), Value = dr["CountryId"].ToString() });

            }
            ViewBag.Country = coutrylist;

        }

        public JsonResult State_Bind(int country_id)
        {

            DataSet dsState = new DataSet();
            dsState = objBalUser.Get_Sate(country_id);
            List<SelectListItem> statelist = new List<SelectListItem>();
            foreach (DataRow dr in dsState.Tables[0].Rows)
            {
                statelist.Add(new SelectListItem { Text = dr["StateName"].ToString(), Value = dr["StateId"].ToString() });
            }
           // ViewBag.StateName = statelist;
            return Json(statelist, JsonRequestBehavior.AllowGet);

        }

        public JsonResult City_Bind(int state_id)
        {

            DataSet ds = new DataSet();
            ds = objBalUser.Get_City(state_id);
            List<SelectListItem> statelist = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                statelist.Add(new SelectListItem { Text = dr["CityName"].ToString(), Value = dr["CityId"].ToString() });
            }
            //ViewBag.cityname = statelist;
            return Json(statelist, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public ActionResult SaveUserD()
        {
            Country_Bind();
            return View();
        }

        [HttpPost]
        public ActionResult SaveUserD(UserDetails userD)
        {
            
            objBalUser.SaveUserDetails(userD.FullName,userD.Gender,userD.MobileNo,userD.DOB, userD.Address,userD.CityId);
            ViewBag.MessageForSave = "Save successfully..!!";
            Country_Bind();
            return View();
        }

        public ActionResult ListUser()
        {
            DataSet dsUserDetails = new DataSet();
            dsUserDetails = objBalUser.GetUserList();
            UserDetails obju = new UserDetails();
            List<UserDetails> list = new List<UserDetails>();
            for (int i = 0; i < dsUserDetails.Tables[0].Rows.Count; i++)
            {
                UserDetails objUserList = new UserDetails();
                objUserList.UserDetailsId = Convert.ToInt32(dsUserDetails.Tables[0].Rows[i]["UserDetailsId"]);
                objUserList.FullName = dsUserDetails.Tables[0].Rows[i]["FullName"].ToString();
                objUserList.MobileNo = dsUserDetails.Tables[0].Rows[i]["MobileNo"].ToString();
                objUserList.Gender = dsUserDetails.Tables[0].Rows[i]["Gender"].ToString();
                objUserList.DOB = Convert.ToDateTime(dsUserDetails.Tables[0].Rows[i]["DOB"]);
                objUserList.Address = dsUserDetails.Tables[0].Rows[i]["Address"].ToString();
                objUserList.CityName = dsUserDetails.Tables[0].Rows[i]["CityName"].ToString();
                list.Add(objUserList);
            }
            return View(list.AsEnumerable());
        }

        public ActionResult EditUserDetails(int id) 
        {
            UserDetails objUserDetails = new UserDetails();
            try
           {
            Country_Bind();
            SqlDataReader dr;
           
            dr = objBalUser.ShowUserDetailsForEdit(id);
                while (dr.Read())
                {
                objUserDetails.UserDetailsId= Convert.ToInt32(dr["UserDetailsId"]);
                objUserDetails.FullName = dr["FullName"].ToString();
                objUserDetails.Address = dr["Address"].ToString();
                objUserDetails.DOB = (DateTime)dr["DOB"];
                objUserDetails.Gender = dr["Gender"].ToString();
                objUserDetails.CityName = dr["CityName"].ToString();
                objUserDetails.CityId = (int)dr["CityId"];
                objUserDetails.CountryName = dr["CountryName"].ToString();
                objUserDetails.CountryId = (int)dr["CountryId"];
                objUserDetails.StateName = dr["StateName"].ToString();
                objUserDetails.StateId = (int)dr["StateId"];
                objUserDetails.MobileNo = dr["MobileNo"].ToString();

                }
               
                ViewBag.cityname = objUserDetails.CityName;
                ViewBag.statename = objUserDetails.StateName;
                ViewBag.cityid = objUserDetails.CityId;
                ViewBag.stateid = objUserDetails.StateId;


            }

            catch (InvalidOperationException)
            {

            }
            return View(objUserDetails);
        }

        [HttpPost]
        public ActionResult EditUserDetails(UserDetails objud)
        {
            Country_Bind();
            objBalUser.UpdateUserDetails(objud.UserDetailsId,objud.FullName,objud.Gender,objud.MobileNo,objud.DOB,objud.Address,objud.CityId);
            ViewBag.msg = "Updated Successfully..!";
            return View();
        }

        public ActionResult UserDetails(int id)
        {
            SqlDataReader dr;
            UserDetails objUserDetails = new UserDetails();
            dr = objBalUser.ShowUserDetailsForEdit(id);
            while (dr.Read())
            {
                objUserDetails.UserDetailsId = Convert.ToInt32(dr["UserDetailsId"]);
                objUserDetails.FullName = dr["FullName"].ToString();
                objUserDetails.Address = dr["Address"].ToString();
                objUserDetails.DOB = (DateTime)dr["DOB"];
                objUserDetails.Gender = dr["Gender"].ToString();
                objUserDetails.CityName = dr["CityName"].ToString();
                objUserDetails.CountryName = dr["CountryName"].ToString();
                objUserDetails.StateName = dr["StateName"].ToString();
                objUserDetails.MobileNo = dr["MobileNo"].ToString();

            }
            return View(objUserDetails);
        }

        public ActionResult DeleteUserDetails(int id)
        {
            objBalUser.DeleteUser(id);
            TempData["msg2"] = "User Deleted Successfully";
            return RedirectToAction("ListUser");
        }
    }
}