using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QUONOW.Models;
using QUONOW.Pattern.UnitOfWork;
using QUONOW.Libraries;

namespace QUONOW.Controllers
{

    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        QUONOWEntities db;
        UnitOfWork unit;
        public UserController()
        {
            this.db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }

        [Route("GetUserList")]
        public IHttpActionResult GetUserList()
        {
            var userList = this.unit._userRepository.SelectAll();
            return Ok(userList);
        }

        [Route("PostSaveUser")]
        public IHttpActionResult PostSaveUser(User user)
        {
            this.unit._userRepository.Save(user);
            var x = this.unit.Commit();
            return Ok(x);
        }


        [Route("GetUserInvoiceList/{token:Guid}")]
        public IHttpActionResult GetUserInvoiceList(Guid token)
        {
            //  List<dynamic> getuserdetails = new List<dynamic>();
            try
            {
                dynamic values = null;
                Utility util = new Utility();

                var getuserId = util.GetUserDetailsByToken(token.ToString()).Id;
                if (getuserId != null)
                {
                    values = unit._bookingRepository.SelectAll()
                             .Where(x => x.UserId == getuserId)
                             .Select(x => new
                             {
                                 invoiceId = x.Id,
                                 productName = this.unit._productRepository.SelectAll().Where(y => y.Id == x.ProductId).FirstOrDefault().ProductName
                             }).ToList();
                    //ass.ForEach(a =>
                    // {
                    //     var booking = new { Id = a.Id };
                    //     getuserdetails.Add(booking);

                    // });
                }
                return Ok(values);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

        }


        [Route("GetUserInvoice/{bookingId:Guid}")]
        public IHttpActionResult GetUserInvoice(Guid bookingId)
        {
            var getInvoiceDetails = this.unit._bookingRepository.SelectAll().Where(x => x.Id == bookingId).Select(x => new { Id = x.Id, ProductId = x.ProductId }).FirstOrDefault();
            if (getInvoiceDetails != null)
            {
                return Ok(getInvoiceDetails);
            }
            return Ok(false);
        }


        [Route("PostAddToCart")]
        public IHttpActionResult PostAddToCart(helperAddtoCart user)
        {
            try
            {
                Utility util = new Utility();
                Cart ca = new Cart();
                ca.Id = Guid.NewGuid();
                ca.productId = user.productId;
                ca.userId = util.GetUserDetailsByToken(user.usertoken).Id;
                ca.IsActive = true;
                ca.IsDeleted = false;
                ca.CreatedOn = DateTime.Now;
                ca.ModifiedOn = DateTime.Now;
                this.unit._cartRepository.Save(ca);
                if (this.unit.Commit() > 0)
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
            }
            return InternalServerError();
        }


        [Route("PostComment")]
        public IHttpActionResult PostComment(helpcomments comments)
        {

            try
            {
                Models.Review rew = new Review();
                Utility util = new Utility();
                rew.Id = Guid.NewGuid();
                rew.Comments = comments.description;
                rew.UserId = new Guid(Convert.ToString(util.GetUserDetailsByToken(comments.token).Id) ?? "00000000-0000-0000-0000-000000000000");
                rew.ProductId = comments.productId;
                rew.CreatedOn = DateTime.Now;
                rew.IsActive = true;
                rew.IsDeleted = false;
                this.unit._reviewRepository.Save(rew);
                this.unit.Commit();
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(true);
        }


        [Route("GetUserCartList/{token:Guid}")]
        public IHttpActionResult GetUserCartList(Guid token)
        {
            try
            {
                Utility util = new Utility();
                var getuserId = util.GetUserDetailsByToken(token.ToString()).Id;
                if (getuserId != null)
                {
                    var getCartDetails = this.unit._cartRepository.SelectAll().Where(x => x.userId == getuserId)
                                         .Select(x => new
                                         {
                                             ProductId = x.productId,
                                             ProductName = this.unit._productRepository.SelectAll().Where(y => y.Id == x.productId).FirstOrDefault().ProductName
                                         }).ToList();
                    return Ok(getCartDetails);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return InternalServerError();
        }

        [Route("GetProfileDetails/{token:Guid}")]
        public IHttpActionResult GetProfileDetails(Guid token)
        {
            try
            {
                Utility util = new Utility();
                var userDetails = util.GetUserDetailsByToken(token.ToString());
                var anonymus = new { Name = userDetails.Name };
                return Ok(anonymus);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }


        [Route("GetKartProducts/{token:Guid}")]
        public IHttpActionResult GetKartProducts(Guid token)
        {
            try
            {
                Utility util = new Utility();
                var getUserDetails = util.GetUserDetailsByToken(token.ToString());
                if (getUserDetails != null)
                {
                    var get = this.unit._cartRepository.SelectAll().Where(x => x.userId == getUserDetails.Id && x.IsDeleted == false)
                             .Select(x => new
                             {
                                 productName = this.unit._productRepository.SelectAll().Where(y => y.Id == x.productId).FirstOrDefault().ProductName,
                                 productId = this.unit._productRepository.SelectAll().Where(y => y.Id == x.productId).FirstOrDefault().Id,
                                 price = this.unit._productRepository.SelectAll().Where(y => y.Id == x.productId).FirstOrDefault().Price,
                             }).ToList();
                    return Ok(get);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }

        }

        //[AuthenticationFilter]
        [Route("GetKartRemoveProduct/{productId:Guid}/{token:Guid}")]
        public IHttpActionResult GetKartRemoveProduct(Guid productId, Guid token)
        {
            try
            {
                Utility util = new Utility();
                var userDetails = util.GetUserDetailsByToken(token.ToString());
                if (userDetails == null)
                {
                    return Ok(false);
                }
                else
                {
                    var cart = this.unit._cartRepository.SelectAll().Where(x => x.productId == productId && x.userId == userDetails.Id).FirstOrDefault();
                    cart.IsDeleted = true;
                    this.unit._cartRepository.Update(cart);
                    return Ok(this.unit.Commit() > 0 ? true : false);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

    }


    public class helpcomments
    {

        public string description { get; set; }

        public string name { get; set; }


        public string email { get; set; }

        public string token { get; set; }

        public Guid productId { get; set; }
    }


    public class helperAddtoCart
    {

        public Guid productId { get; set; }

        public string usertoken { get; set; }

    }
}
