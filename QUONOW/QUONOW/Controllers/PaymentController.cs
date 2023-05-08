using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Stripe;
using QUONOW.Models;
using QUONOW.Pattern.UnitOfWork;
using QUONOW.Libraries;

namespace QUONOW.Controllers
{
    [RoutePrefix("api/Payment")]
    public class PaymentController : ApiController
    {
        QUONOWEntities db;
        UnitOfWork unit;

        public PaymentController()
        {
            this.db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }

        [Route("PostMakePayment")]
        public IHttpActionResult PostMakePayment(Customer customer)
        {
            
            Utility util = new Utility();
            customer.Amount = util.GetProductPrice(new Guid(customer.productId));
            Booking book = new Booking();
            var guid = Guid.NewGuid();
            var getUserDetails = util.GetUserDetailsByToken(customer.userToken);
            book.Id = guid;
            book.PubId = null;
            book.EventId = null;
            book.ProductId = new Guid(customer.productId);
            book.UserId = getUserDetails.Id;
            book.IsActive = true;
            book.IsDeleted = false;
            book.CreatedOn = DateTime.Now;
            book.ModifiedOn = DateTime.Now;
            if (customer.Payment())
            {
                try
                {
                    this.unit._bookingRepository.Save(book);
                    this.unit.Commit();
                    Libraries.Email email = new Libraries.Email();
                    email.To = getUserDetails.Email;
                    email.Subject = "Payment Successfully";
                    email.Body = customer.Amount.ToString() + " has taken successfully";
                    email.SendEmail();
                    //Libraries.Sms sms = new Sms();
                    //sms.PhoneNoTo = 9791177156;
                    //sms.Message = "Payment Successfully";
                    //sms.SendSms();
                }
                catch (Exception ex)
                {
                    ErrorLog.Log(ex);
                }
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [Route("GetPaymentList")]
        public IHttpActionResult GetPaymentList()
        {
            var paymentList = this.unit._bookingRepository.SelectAll();
            return Ok(paymentList);
        }

        [Route("GetUserPayments/{userId:Guid}")]
        public IHttpActionResult GetUserPayments(Guid userId)
        {
            var userpayments = this.unit._bookingRepository.SelectAll().Select(x => x.UserId == userId).ToList();
            return Ok(userpayments);
        }


        [Route("PostMultiplePayment")]
        public IHttpActionResult PostMultiplePayment(List<Customer> listCustomer)
        {
            try
            {
                int i = 0;
                Boolean isPay = false;
                int totalPayment = 0;
                listCustomer.ForEach(x =>
                {
                    i++;
                    Utility util = new Utility();
                    var userDetails = util.GetUserDetailsByToken(x.userToken);
                    totalPayment = totalPayment + util.GetProductPrice(new Guid(x.productId));
                    var getUserDetails = util.GetUserDetailsByToken(x.userToken);
                    Booking book = new Booking();
                    var guid = Guid.NewGuid();
                    book.Id = guid;
                    book.PubId = null;
                    book.EventId = null;
                    book.ProductId = new Guid(x.productId);
                    book.UserId = getUserDetails.Id;
                    book.IsActive = true;
                    book.IsDeleted = false;
                    book.CreatedOn = DateTime.Now;
                    book.ModifiedOn = DateTime.Now;
                    this.unit._bookingRepository.Save(book);
                    this.unit.Commit();
                    if (listCustomer.Count == i)
                    {
                        x.Amount = totalPayment;
                        if (x.Payment())
                        {
                            Libraries.Email email = new Libraries.Email();
                            email.To = getUserDetails.Email;
                            email.Subject = "Payment Successfully";
                            email.Body = x.Amount.ToString() + " has taken successfully";
                            email.SendEmail();
                            //Libraries.Sms sms = new Sms();
                            //sms.PhoneNoTo = 9791177156;
                            //sms.Message = "Payment Successfully";
                            //sms.SendSms();
                        }
                    }

                });

                return Ok(isPay);
            }
            catch (Exception ex)
            {

                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }


    }
}
