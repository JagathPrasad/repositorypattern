using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QUONOW.Pattern.UnitOfWork;
using QUONOW.Models;
using QUONOW.Libraries;


namespace QUONOW.Controllers
{
    [RoutePrefix("api/Admin")]
    public class AdminController : ApiController
    {
        private UnitOfWork unit;
        public QUONOWEntities db;

        public AdminController()
        {
            this.db = new QUONOWEntities();
            this.unit = new UnitOfWork(this.db);
        }

        [Route("GetUserList")]
        public IHttpActionResult GetUserList()
        {
            List<User1> user = new List<User1>();
            try
            {
                var userList = this.unit._userRepository.SelectAll().Where(x => x.IsValidated == false).ToList();
                userList.ForEach(a =>
               {
                   User1 us = new User1();
                   us.Id = a.Id;
                   us.Email = a.Email;
                   us.IsActive = a.IsActive;
                   user.Add(us);
               });
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(user);
        }

        [Route("GetEventsTypesList")]
        public IHttpActionResult GetEventsTypesList()
        {
            List<Events> e = new List<Events>();
            var events = this.unit._evnetTypeRepository.SelectAll();
            events.ForEach(x =>
            {
                Events eve = new Events();
                eve.Id = x.Id;
                eve.Type = x.Type;
                e.Add(eve);
            });
            return Ok(e);
        }

        [Route("GetEvents/{eventTypeId:Guid}")]
        [AuthenticationFilter]
        public IHttpActionResult GetEvents(Guid eventTypeId)
        {
            List<dynamic> dyList = new List<dynamic>();
            var events = this.unit._eventRepository.SelectAll().Where(x => x.EventType == eventTypeId).ToList();
            events.ForEach(x =>
            {
                var a = new { Id = x.Id, Name = x.EventName };
                dyList.Add(a);
            });
            return Ok(dyList);
        }


        [Route("GetProductType/{eventId:Guid}")]
        public IHttpActionResult GetProductType(Guid eventId)
        {
            List<dynamic> listdynamic = new List<dynamic>();
            var producttypes = unit._productTypeRepository.SelectAll().Where(x => x.EventId == eventId).ToList();
            producttypes.ForEach(a =>
            {
                var product = new { Id = a.Id, Type = a.Type };
                listdynamic.Add(product);
            });
            return Ok(listdynamic);
        }


        [Route("GetProductList")]
        public IHttpActionResult GetProductList()
        {
            List<dynamic> listdynamic = new List<dynamic>();
            try
            {
                var getproductDetails = this.unit._productRepository.SelectAll().Where(x => x.IsValidated == false).ToList();
                getproductDetails.ForEach(a =>
                {
                    dynamic product = new { ProductName = a.ProductName, Id = a.Id };
                    listdynamic.Add(product);
                });
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(listdynamic);
        }


        [Route("AddProductType")]
        public IHttpActionResult AddProductType(ProductType productType)
        {
            try
            {
                var guid = Guid.NewGuid();
                Models.ProductType prod = new Models.ProductType();
                prod.Id = guid;
                prod.IsActive = true;
                prod.IsDeleted = false;
                prod.Type = productType.Type;
                prod.EventId = productType.EventId;
                //productType.Id = Guid.NewGuid();
                //productType.IsActive = true;
                //productType.IsDeleted = false;
                this.unit._productTypeRepository.Save(prod);
                if (this.unit.Commit() > 0)
                {
                    return Ok(guid);
                }
                else
                {
                    return Ok("");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }


        [Route("PostAdminApprove")]
        public IHttpActionResult PostAdminApprove(User1 user)
        {
            try
            {
                var userDetails = this.unit._userRepository.SelectAll().Where(x => x.Id == user.Id).FirstOrDefault();
                userDetails.IsValidated = true;
                this.unit._userRepository.Update(userDetails);
                this.unit.Commit();

            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(true);
        }


        [Route("GetProductDetails/{productId:Guid}")]
        public IHttpActionResult GetProductDetails(Guid productId)
        {
            dynamic y = null;
            try
            {
                y = this.unit._productRepository.SelectAll().Join(this.unit._userRepository.SelectAll(), P => P.VendorId, U => U.Id, (P, U) => new { P, U })
                    .Where(x => x.P.Id == productId && x.U.IsValidated == true)
                    .Select(x => new { UserName = x.U.Name, Star = 3, Price = x.P.Price })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(y);
        }


        [Route("PostApproveProduct")]
        public IHttpActionResult PostApproveProduct(Products product)
        {
            try
            {
                var x = unit._productRepository.Select(product.Id);
                x.IsValidated = true;
                unit._productRepository.Update(x);
                unit.Commit();
            }
            catch (Exception ex)
            {

            }
            return Ok(true);
        }


        [Route("GetReviewsList")]
        public IHttpActionResult GetReviewsList()
        {
            try
            {
                return Ok(this.unit._reviewRepository.SelectAll().Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => new { Comments = x.Comments, Star = x.Star, Id = x.Id }).ToList());
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError();
            }
        }

        [Route("GetApproveReview/{Id:Guid}/{Isdeleted:bool}")]
        public IHttpActionResult GetApproveReview(Guid Id, bool Isdeleted)
        {
            try
            {
                var x = this.unit._reviewRepository.Select(Id);
                if (!Isdeleted)
                {
                    x.IsDeleted = true;
                }
                else if (Isdeleted)
                {
                    x.IsActive = false;
                }
                x.ModifiedOn = DateTime.Now;
                this.unit._reviewRepository.Update(x);
                this.unit.Commit();
                return Ok(true);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError();
            }



        }


        [Route("GetChartValues")]
        public IHttpActionResult GetChartValues()
        {

            try
            {

                var getProductDetails = this.unit._evnetTypeRepository.SelectAll();
            }
            catch (Exception ex)
            {
            }

            return Ok();
        }


        [Route("GetAllUsers")]
        public IHttpActionResult GetAllUsers()
        {
            try
            {
                var userList = this.unit._userRepository.SelectAll().Select(x => new
                {
                    Name = x.Name,
                    Email = x.Email
                }).ToList();

                return Ok(userList);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError();
            }

        }


        [Route("GetTicketDetailsList")]
        public IHttpActionResult GetTicketDetailsList()
        {
            try
            {
                var getTicketDetailsList = this.unit._PubRepository.SelectAll()
                                           .Select(x => new
                                           {
                                               Id = x.Id,
                                               pubName = x.Name,
                                               startTime = x.Starttime,
                                               endTime = x.Endtime
                                           }).ToList();
                return Ok(getTicketDetailsList);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }


        [Route("GetBoughtTicketDetails")]
        public IHttpActionResult GetBoughtTicketDetails()
        {
            try
            {
                var getboughDetails = this.unit._bookingRepository.SelectAll()
                                      .Join(this.unit._userRepository.SelectAll(), B => B.UserId, U => U.Id, (B, U) => new { B, U })
                                      .Where(x => x.B.PubId != null && x.B.EventId == null)
                                      .Select(x => new
                                      {
                                          pubName = this.unit._PubRepository.SelectAll().Where(y => y.Id == x.B.PubId).FirstOrDefault().Name,
                                          userName = x.U.Name
                                      }).ToList();
                return Ok(getboughDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }


        [Route("PostSavePub")]
        public IHttpActionResult PostSavePub(HelperPubDetails pub)
        {
            try
            {
                var guid = Guid.NewGuid();
                Pub pubDetails = new Pub();
                pubDetails.Id = guid;
                pubDetails.Name = pub.Name;
                pubDetails.Location = pub.Location;
                pubDetails.Price = pub.Price;
                pubDetails.Star = pub.Star;
                pubDetails.Phone = pub.Phone;
                pubDetails.Total = pub.Total;
                pubDetails.Stag = pub.Stag;
                pubDetails.Couple = pub.Couple;
                pubDetails.Starttime = Convert.ToDateTime(pub.Starttime);
                pubDetails.Endtime = Convert.ToDateTime(pub.Endtime);
                pubDetails.IsActive = true;
                pubDetails.IsDeleted = false;
                pubDetails.Organizer = pub.Organiser;// "Titos Testing data";
                this.unit._PubRepository.Save(pubDetails);
                if (this.unit.Commit() > 0)
                {
                    return Ok(guid);
                }
                else
                {

                    return Ok("");
                }

            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }

        [Route("GetSalesDetails")]
        public IHttpActionResult GetSalesDetails()
        {
            try
            {
                var booking = this.unit._bookingRepository.SelectAll().Where(x => x.ProductId != null).OrderByDescending(x => x.CreatedOn).ToList();
                booking.ForEach(a =>
                {


                });
            }
            catch (Exception ex)
            {

            }

            return Ok();
        }


        [Route("GetApproveReject/{isApprove:bool}/{isReject:bool}/{productId:Guid}")]
        public IHttpActionResult GetApproveReject(bool isApprove, bool isReject, Guid productId)
        {
            try
            {
                if (isApprove && !isReject)
                {
                    var getproduct = this.unit._productRepository.SelectAll().Where(x => x.Id == productId).FirstOrDefault();
                    this.unit._productRepository.Update(getproduct);
                    this.unit.Commit();
                }
                else if (isReject && !isApprove)
                {
                    var getproduct = this.unit._productRepository.SelectAll().Where(x => x.Id == productId).FirstOrDefault();
                    this.unit._productRepository.Update(getproduct);
                    this.unit.Commit();
                }
            }
            catch (Exception ex)
            {

            }


            return Ok();
        }

        void ConvertCoommon(dynamic a)
        {
            foreach (var b in a)
            {
                var x = a.GetType().GetProperty("");
            }

        }

        [Route("GetCountDetails")]
        public IHttpActionResult GetCountDetails()
        {
            try
            {
                DateTime dateTime = DateTime.Now.Date;
                var objects = new
                {
                    user = this.unit._userRepository.SelectAll().Where(x => x.CreatedOn <= dateTime).Count(),
                    tickets = this.unit._bookingRepository.SelectAll().Where(x => x.CreatedOn <= dateTime && x.PubId != null).Count(),
                    events = this.unit._bookingRepository.SelectAll().Where(x => x.CreatedOn <= dateTime && x.EventId != null).Count()
                };//anonymus types.
                return Ok(objects);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }

        }


    }


    public class User1
    {

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }


        public bool? IsActive { get; set; }



    }
    public class Events
    {
        public Guid Id { get; set; }

        public string Type { get; set; }



    }
    public class Products
    {

        public Guid Id { get; set; }

    }
    public class ProductType
    {
        public System.Guid Id { get; set; }
        public string Type { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.Guid> EventId { get; set; }
        public string image { get; set; }
    }

    public class HelperPubDetails
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public decimal Price { get; set; }

        public int Total { internal get; set; }

        public int Star { internal get; set; }

        public string Description { get; set; }

        public decimal Stag { get; set; }

        public decimal Couple { get; set; }

        public DateTime Starttime { get; set; }

        public DateTime Endtime { get; set; }

        public string Phone { get; set; }

        public string Organiser { get; set; }

    }
}
