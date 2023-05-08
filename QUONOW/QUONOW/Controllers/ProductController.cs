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

    [RoutePrefix("api/Product")]
    public class ProductController : ApiController
    {
        QUONOWEntities db;
        UnitOfWork unit;
        public ProductController()
        {
            this.db = new QUONOWEntities();
            this.unit = new UnitOfWork(db);
        }

        [Route("GetProductList/{productId:Guid}")]
        public IHttpActionResult GetProductList(Guid productId)
        {
            try
            {
                var productsList = this.unit._productRepository.SelectAll()
                                  .Where(x => x.ProductTypeId == productId && x.IsValidated == true)
                                  .Select(x => new
                                  {
                                      Id = x.Id,
                                      IsActive = x.IsActive,
                                      ProductTypeId = x.ProductTypeId,
                                      ProductName = x.ProductName,
                                      Price = x.Price,
                                      Description = x.Description
                                  }).ToList();

                var getcurrentEventType = this.unit._evnetTypeRepository.SelectAll().Where(a => a.Id == this.unit._eventRepository.SelectAll().Where(z => z.Id ==
                          this.unit._productTypeRepository.SelectAll().Where(y => y.Id == productId).FirstOrDefault().EventId).FirstOrDefault().EventType).FirstOrDefault().Id;

                var productsList1 = this.unit._productRepository.SelectAll().Join(this.unit._evnetTypeRepository.SelectAll(), P => P.EventTypeId, ET => ET.Id, (P, ET) => new { P, ET })
                                .Where(x => x.P.ProductTypeId == productId && x.ET.Id == getcurrentEventType && x.P.IsValidated == true)
                                .Select(x => new
                                {
                                    Id = x.P.Id,
                                    IsActive = x.P.IsActive,
                                    ProductTypeId = x.P.ProductTypeId,
                                    ProductName = x.P.ProductName,
                                    Price = x.P.Price,
                                    Description = x.P.Description
                                }).ToList();

                return Ok(productsList1);
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
                return InternalServerError(ex);

            }

        }

        [Route("GetProductsTypeList/{productTypeId:Guid}")]
        public IHttpActionResult GetProductsTypeList(Guid productTypeId)
        {
            dynamic producttypesList = null;
            try
            {
                producttypesList = this.unit._productTypeRepository.SelectAll()
                                          .Where(x => x.EventId == productTypeId)
                          .Select(x => new { Type = x.Type, Id = x.Id }).ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);

            }
            return Ok(producttypesList);
        }



        [Route("PostSaveProduct")]
        public IHttpActionResult PostSaveProduct()
        {
            return Ok();
        }

        [Route("GetUserProduct/{userId:Guid}")]
        public IHttpActionResult GetUserProduct(Guid userId)
        {

            return Ok();
        }


        [Route("GetProductDescription/{productId:Guid}")]
        public IHttpActionResult GetProductDescription(Guid productId)
        {
            dynamic productDetails = null;
            try
            {
                productDetails = this.unit._productRepository.SelectAll().Where(x => x.Id == productId).Select(x => new { productId = x.Id, price = x.Price, description = x.Description }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(productDetails);
        }




        [Route("GetProductComments/{productId:Guid}")]
        public IHttpActionResult GetProductComments(Guid productId)
        {
            dynamic comments = null;
            try
            {
                comments = this.unit._reviewRepository.SelectAll()
                    .Where(x => x.ProductId == productId && x.IsActive == false && x.IsDeleted == false)
                    .Select(x => new { comments = x.Comments, star = x.Star, userId = x.UserId }).ToList();
            }
            catch (Exception ex)
            {
                ErrorLog.Log(ex);
            }
            return Ok(comments);
        }

        [Route("PostProductByName")]
        public IHttpActionResult PostProductByName(dynamic product)
        {
            try
            {
                string searchText = product["usertext"].Value;
                if (!string.IsNullOrEmpty(searchText))
                {
                    var getProductDetails = this.unit._productRepository.SelectAll().Where(x => x.ProductName.ToLower().Contains(searchText.ToLower()))
                                           .Select(x => new { Id = x.Id, productName = x.ProductName, price = x.Price }).ToList();
                    return Ok(getProductDetails);
                }
                return Ok("success");
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }


        }

    }


    public class HelpProductTyps
    {
        public System.Guid Id { get; set; }
        public string Type { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }


    public class HelpProducts
    {

        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public Guid? ProductTypeId { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }


    }

}
