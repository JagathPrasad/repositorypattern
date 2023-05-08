using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QUONOW.Pattern.UnitOfWork;
using QUONOW.Models;
using QUONOW.Libraries;
using System.Web;
using System.Web.Http.Results;

namespace QUONOW.Controllers
{
    [RoutePrefix("api/Event")]
    public class EventController : ApiController
    {
        QUONOWEntities db;
        UnitOfWork unit;

        public EventController()
        {
            this.db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);

        }

        [Route("GetEventsList")]
        public IHttpActionResult GetEventsList()
        {
            List<EventType> eventList = new List<EventType>();
            try
            {
                var eventsList = this.unit._evnetTypeRepository.SelectAll();
                eventsList.ForEach(x =>
                {
                    EventType even = new EventType();
                    if (Convert.ToBoolean(x.IsActive))
                    {
                        even.Id = x.Id;
                        even.Type = x.Type;
                    }
                    eventList.Add(even);
                });
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(eventList);
        }


        [Route("PostSaveEvent")]
        public IHttpActionResult PostSaveEvent()
        {


            return Ok();
        }

        [Route("GetUserEvents/{userId:Guid}")]
        public IHttpActionResult GetUserEvents(Guid userId)
        {
            var getuserEvents = this.unit._bookingRepository.SelectAll().Where(x => x.UserId == userId).Select(y => new
            {
                UserId = y.UserId,
                EventName = this.unit._eventRepository.Select(y.EventId).EventName,
                ProductName = this.unit._productRepository.Select(y.ProductId).ProductName
            }).ToList();

            return Ok();
        }


        [Route("GetEventsTypesList/{eventTypeId:Guid}")]
        public IHttpActionResult GetEventsTypesList(Guid eventTypeId)
        {
            try
            {
                var getEventList = this.unit._eventRepository.SelectAll()
                                   .Where(x => x.EventType == eventTypeId)
                                  .Select(x => new { EventNames = x.EventName, Id = x.Id }).ToList();
                return Ok(getEventList);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }
        }

        [Route("PostAddEventType")]
        [AuthenticationFilter]
        public IHttpActionResult PostAddEventType(Models.EventType events)
        {
            try
            {
                var newGuid = Guid.NewGuid();
                //HttpResponseMessage response = new HttpResponseMessage();
                //var httpRequest = HttpContext.Current.Request;
                //if (httpRequest.Files.Count > 0)
                //{
                //    foreach (string file in httpRequest.Files)
                //    {
                //        var postFile = httpRequest.Files[file];
                //        //postFile.FileName = newGuid.ToString() + ".jpg" +;
                //        var filePath = HttpContext.Current.Server.MapPath("~/Gallery/" + newGuid.ToString() + ".jpg");
                //        postFile.SaveAs(filePath);
                //    }

                //}
                events.Id = newGuid;
                events.IsActive = true;
                events.IsDeleted = false;
                this.unit._evnetTypeRepository.Save(events);
                if (this.unit.Commit() > 0)
                {
                    return Ok(newGuid);
                }
                else
                {
                    return Ok("");
                }
            }
            catch (Exception ex)
            {
                return Ok("");
            }

        }


        [Route("PostAddEvent")]
        [AuthenticationFilter]
        public IHttpActionResult PostAddEvent(Event events)
        {
            try
            {
                var guid = Guid.NewGuid();
                Models.Event eve = new Models.Event();
                eve.Id = guid;
                eve.EventName = events.Name;
                eve.EventType = events.EventTypeId;
                eve.IsActive = true;
                eve.IsDeleted = false;
                eve.ModifiedOn = DateTime.Now;
                eve.CreatedOn = DateTime.Now;
                this.unit._eventRepository.Save(eve);
                if (this.unit.Commit() > 0)
                {
                    return Ok(guid);
                }
                else { return Ok(""); }
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);
            }

        }



        [Route("GetSearchKeyword")]
        public IHttpActionResult GetSearchKeyword(string keyword)
        {
            try
            {

                //y = this.unit._productRepository.SelectAll().Join(this.unit._userRepository.SelectAll(), P => P.VendorId, U => U.Id, (P, U) => new { P, U })
                //    .Where(x => x.P.Id == productId && x.U.IsValidated == true)
                //    .Select(x => new { UserName = x.U.Name, Star = 3, Price = x.P.Price })
                //    .FirstOrDefault();
                var output = new
                {
                    events = this.unit._eventRepository.SelectAll().Where(x => x.EventName.Contains(keyword)).ToList(),
                    pubs = this.unit._PubRepository.SelectAll().Where(y => y.Name.Contains(keyword)).ToList()
                };
                return Ok(output);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

    }
    #region Helper Class

    public class EventType
    {
        public System.Guid Id { get; set; }
        public string Type { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string EventNames { get; set; }

    }


    public class Event
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid EventTypeId { get; set; }



    }
    #endregion
}
