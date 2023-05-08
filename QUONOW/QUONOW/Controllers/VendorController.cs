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
    [RoutePrefix("api/Vendor")]
    public class VendorController : ApiController
    {

        private UnitOfWork unit;

        public VendorController()
        {
            QUONOWEntities db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }

        [Route("GetUserApproval/{VendorId:Guid}")]
        public IHttpActionResult GetUserApproval(Guid VendorId)
        {
            dynamic getuserapprovalproducts = null;
            try
            {
                getuserapprovalproducts = unit._productRepository.SelectAll()
                                          .Where(x => x.VendorId == VendorId).Select(x => new { Id = x.Id, ProductName = x.ProductName }).ToList();
            }
            catch (Exception ex) { }
            return Ok(getuserapprovalproducts);
        }


        [Route("PostSaveProduct")]
        public IHttpActionResult PostSaveProduct(Models.Product product)
        {
            try
            {
                var guid = Guid.NewGuid();
                Utility util = new Utility();
                var userDetisl = util.GetUserDetailsByToken(product.VendorId.ToString());
                if (userDetisl != null)
                {
                    product.EventTypeId = product.Id;
                    product.Id = guid;
                    product.IsActive = true;
                    product.IsDeleted = false;
                    product.VendorId = userDetisl.Id;
                    product.IsValidated = false;
                    product.CreatedOn = DateTime.Now;
                    product.ModifiedOn = DateTime.Now;
                    this.unit._productRepository.Save(product);
                    if (this.unit.Commit() > 0)
                    {
                        return Ok(guid);
                    }
                    else
                    {
                        return Ok("");
                    }
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



        [Route("GetVendorProductsList/{VendorId:Guid}")]
        public IHttpActionResult GetVendorProductsList(Guid VendorId)
        {
            try
            {
                var getuserapprovalproducts = unit._productRepository.SelectAll()
                                 .Where(x => x.VendorId == VendorId).Select(x => new { Id = x.Id, ProductName = x.ProductName }).ToList();
                return Ok(getuserapprovalproducts);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return InternalServerError();
        }


        [Route("GetUpComingEvents/{VendorId:Guid}")]
        public IHttpActionResult GetUpComingEvents(Guid VendorId)
        {
            try
            {
                var getupcomingDetails = this.unit._bookingRepository.SelectAll()
                                         .Join(this.unit._productRepository.SelectAll(), B => B.ProductId, P => P.Id, (B, P) => new { B, P }).
                                         Where(x => x.P.VendorId == VendorId && x.P.IsActive == true && x.B.IsActive == true).
                                         Select(x => new
                                         {
                                             userName = this.unit._userRepository.SelectAll().Where(y => y.Id == x.B.UserId).FirstOrDefault().Name
                                         ,
                                             EventName = this.unit._eventRepository.SelectAll().Where(z => z.Id == x.B.EventId).FirstOrDefault().EventName
                                         }).ToList();


                return Ok(getupcomingDetails);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError();
            }

        }

    }
}
