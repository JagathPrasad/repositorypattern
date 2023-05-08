using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QUONOW.Pattern.UnitOfWork;
using QUONOW.Libraries;
using QUONOW.Models;

namespace QUONOW.Controllers
{

    [RoutePrefix("api/Ticket")]
    public class TicketController : ApiController
    {
        UnitOfWork unit;



        public TicketController()
        {
            QUONOWEntities db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }


        [Route("GetTicketDetailsById/{userId:Guid}")]
        public IHttpActionResult GetTicketDetailsById(Guid userId)
        {
            try
            {
                var getTicketDetails = this.unit._bookingRepository.SelectAll().Where(x => x.UserId == userId && x.EventId == null && x.ProductId == null)
                                     .Select(x => new
                                     {
                                         Name = x.PubId != null ? this.unit._PubRepository.SelectAll().Where(y => y.Id == x.PubId).FirstOrDefault().Name : "",
                                         Time = x.CreatedOn,
                                         Id = x.Id
                                     }).ToList();
                return Ok(getTicketDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }


        [Route("GetPubDetails")]
        public IHttpActionResult GetPubDetails()
        {
            try
            {
                var getpubDetails = this.unit._PubRepository.SelectAll()
                                    .Where(x => x.IsDeleted == false).Select(
                                     x => new
                                     {
                                         Name = x.Name,
                                         Location = x.Location,
                                         Id = x.Id,
                                         Time = Convert.ToDateTime(x.Starttime).Hour,
                                         Date = x.Starttime
                                     }).ToList();
                return Ok(getpubDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }


        [Route("PostSaveTickets")]
        public IHttpActionResult PostSaveTickets(Models.Booking book)
        {
            try
            {
                bool IsSave = false;
                var guid = Guid.NewGuid();
                book.Id = guid;
                this.unit._bookingRepository.Save(book);
                IsSave = this.unit.Commit() > 0 ? true : false;
                var pubDetails = this.unit._PubRepository.SelectAll().Where(x => x.Id == book.PubId).FirstOrDefault();
                pubDetails.Total = pubDetails.Total != 0 ? pubDetails.Total - 1 : pubDetails.Total;
                this.unit._PubRepository.Update(pubDetails);
                this.unit.Commit();
                return Ok(IsSave);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }


        [Route("DeletePubDetails/{pubId:Guid}")]
        public IHttpActionResult DeletePubDetails(Guid pubId)
        {

            try
            {
                bool IsSave = false;
                var pubs = this.unit._PubRepository.SelectAll().Where(x => x.Id == pubId).FirstOrDefault();
                pubs.IsDeleted = true;
                this.unit._PubRepository.Update(pubs);
                IsSave = this.unit.Commit() > 0 ? true : false;
                return Ok(IsSave);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }

        }


        [Route("GetPubDetailsById/{pubId:Guid}")]
        public IHttpActionResult GetPubDetailsById(Guid pubId)
        {
            try
            {
                var pubDetails = this.unit._PubRepository.SelectAll().Where(x => x.Id == pubId && x.IsDeleted == false)
                              .Select(x => new
                              {
                                  name = x.Name,
                                  location = x.Location,
                                  price = x.Price,
                                  stag = x.Stag,
                                  couple = x.Couple,
                                  contact = x.Phone,
                                  startTime = x.Starttime,
                                  endTime = x.Endtime,
                                  description = x.Description,
                                  organizer = x.Organizer
                              }).FirstOrDefault();

                return Ok(pubDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }

        [Route("PostTicketPayment")]
        public IHttpActionResult PostTicketPayment(Customer customer)
        {
            Booking book = new Booking();
            var guid = Guid.NewGuid();
            Utility util = new Utility();
            var getUserDetails = util.GetUserDetailsByToken(customer.userToken);
            book.Id = guid;
            book.PubId = new Guid(customer.pubId);
            book.EventId = null;
            book.ProductId = null;
            book.UserId = getUserDetails.Id;
            book.IsActive = true;
            book.IsDeleted = false;
            book.CreatedOn = DateTime.Now;
            book.ModifiedOn = DateTime.Now;
            var pubDetails = this.unit._PubRepository.SelectAll().Where(x => x.Id == book.PubId).FirstOrDefault();
            customer.Amount = Convert.ToInt32((pubDetails.Stag * Convert.ToInt32(customer.single)) + (pubDetails.Couple * Convert.ToInt32(customer.couple)));
            if (pubDetails.Total >= (Convert.ToInt32(customer.single) + Convert.ToInt32(customer.couple)))
            {
                pubDetails.Total = pubDetails.Total != 0 ? pubDetails.Total - (Convert.ToInt32(customer.single) + Convert.ToInt32(customer.couple)) : pubDetails.Total;
                if (customer.Payment())
                {
                    try
                    {
                        this.unit._bookingRepository.Save(book);
                        this.unit.Commit();
                        this.unit._PubRepository.Update(pubDetails);
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
            else
            {
                return Ok(false);
            }

        }

    }
}
