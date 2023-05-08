

namespace QUONOW.Controllers
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web;
    using System.Web.Http.Results;
    using QUONOW.Models;
    using QUONOW.Pattern.UnitOfWork;
    using Libraries;
    #endregion



    [RoutePrefix("api/Login")]
    public class LoginController : ApiController
    {
        QUONOWEntities db;
        UnitOfWork uoW;
        LoginController()
        {
            this.db = new QUONOWEntities();
            this.uoW = new UnitOfWork(db);
        }


        [Route("PostUserLogin")]
        public IHttpActionResult PostUserLogin(LoginHelper login)
        {
            HelpUser user = new HelpUser();
            var check = this.uoW._userRepository.SelectAll().Where(x => x.Email == login.username && x.Password == login.password && x.IsValidated == true).FirstOrDefault();
            if (check != null)
            {
                user.email = check.Email;
                user.token = check.Token;
                user.userType = check.UserTypeId;
                return Ok(user);
            }

            //this.uoW._bookingRepository.Select(new Guid(""));
            return Ok(false);
        }


        [Route("GetUserToken")]
        public IHttpActionResult GetUserToken()
        {
            return Ok();
        }


        [Route("PostRegistration")]
        public IHttpActionResult PostRegistration(Models.User login)
        {
            login.CreatedOn = DateTime.Now;
            login.ModifiedOn = DateTime.Now;
            var Id = Guid.NewGuid();
            login.Token = Id.ToString();
            login.Id = Id;
            login.IsValidated = login.UserTypeId == 2 ? false : true;
            this.uoW._userRepository.Save(login);
            if (this.uoW.Commit() > 0)
            {
                Libraries.Email email = new Libraries.Email();
                email.To = login.Email;
                email.Subject = "Registerd Successfully";
                email.Body = "Welcome to Quonow.";
                email.SendEmail();
                //Libraries.Sms sms = new Sms();
                //sms.PhoneNoTo = 9791177156;
                //sms.Message = "Registerd Successfully";
                //sms.SendSms();
                return Ok(true);
            }
            return Ok(false);
        }


        [Route("GetUserExist")]
        public IHttpActionResult GetUserExist(string email)
        {
            bool isTrue = false;
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    var y = this.uoW._userRepository.SelectAll().Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
                    if (y != null)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        isTrue = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isTrue = false;
            }

            return Ok(isTrue);
        }


        [Route("GetChangePassword")]
        public IHttpActionResult GetChangePassword(string password, string email)
        {
            try
            {
                var getuserDetails = this.uoW._userRepository.SelectAll().Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
                getuserDetails.Password = password;
                this.uoW._userRepository.Save(getuserDetails);
                this.uoW.Commit();
            }
            catch (Exception ex)
            {
            }
            return Ok(true);
        }

        [Route("PostUploadJsonFile")]
        public IHttpActionResult PostUploadJsonFile()
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        var filePath = HttpContext.Current.Server.MapPath("~/Gallery/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }



        [Route("PostUploadFormDate")]
        public IHttpActionResult PostUploadFormDate(Guid Id)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        string fileName = Id + ".jpg";
                        var filePath = HttpContext.Current.Server.MapPath("~/Gallery/" + fileName);
                        postedFile.SaveAs(filePath);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }



        [Route("PostLoginFG")]
        public IHttpActionResult PostLoginFG(LoginHelper user)
        {
            try
            {
                dynamic userDetails = null;
                var userExist = this.uoW._userRepository.SelectAll().Where(x => x.Email == user.username && x.Password == user.password && x.IsDeleted == false).FirstOrDefault();
                if (userExist == null)
                {
                    var token = Guid.NewGuid();
                    Models.User user1 = new Models.User();
                    user1.Id = token;
                    user1.Email = user.username;
                    user1.Password = user.password;
                    user1.UserTypeId = 3;
                    user1.IsValidated = true;
                    user1.Name = "Testing";
                    user1.IsActive = true;
                    user1.IsDeleted = false;
                    user1.CreatedOn = DateTime.Now;
                    user1.ModifiedOn = DateTime.Now;
                    user1.Token = token.ToString();
                    this.uoW._userRepository.Save(user1);
                    if (this.uoW.Commit() > 0)
                    {
                        userDetails = new { email = user1.Email, token = user1.Token, userType = user1.UserTypeId };
                        //userDetails.email = user1.Email;
                        //userDetails.token = user1.Token;
                        //userDetails.userType = user1.UserTypeId;
                        return Ok(userDetails);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    userDetails = new { email = userExist.Email, token = userExist.Token, userType = userExist.UserTypeId };
                    //userDetails.email = userExist.Email;
                    //userDetails.token = userExist.Token;
                    //userDetails.userType = userExist.UserTypeId;
                    return Ok(userDetails);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }

        }


        //protected override OkResult Ok()
        //{
        //    return base.Ok();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //}

    }

    public class LoginHelper
    {

        public string username
        {
            get;
            set;
        }

        public string password { get; set; }
    }


    public class HelpUser
    {
        public string email { get; set; }
        public string token { get; set; }
        public int? userType { get; set; }

    }
}
