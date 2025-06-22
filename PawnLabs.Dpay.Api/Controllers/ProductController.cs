using Microsoft.AspNetCore.Mvc;
using PawnLabs.Dpay.Api.Controllers.Base;
using PawnLabs.Dpay.Api.Model.Request;
using PawnLabs.Dpay.Api.Model.Response;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Core.Entity;

namespace PawnLabs.Dpay.Api.Controllers
{
    public class ProductController : BaseApiController
    {
        private IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Route("/product/all")]
        public async Task<IActionResult> GetAll()
        {
            var response = new List<ProductResponseModel>();

            try
            {
                if (string.IsNullOrEmpty(Email))
                    return Unauthorized();

                var products = await _productService.GetAll(new Product() { Email = Email });

                if(products != null && products?.Count > 0)
                {
                    foreach(var product in products)
                    {
                        response.Add(new ProductResponseModel()
                        {
                            ID = product.ID,
                            Name = product.Name,  
                            Description = product.Description,
                            Price = product.Price,
                            PriceType = product.PriceType,
                            Logo = product.Logo,
                            CreationDate = product.CreationDate
                        });
                    }
                }

                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("/product/{productID}")]
        public async Task<IActionResult> Get(Guid productID)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                    return Unauthorized();

                if(productID == Guid.Empty)
                    return BadRequest();

                var product = await _productService.Get(new Product() { ID = productID, Email = Email });

                if (product == null)
                    return NotFound();

                return Ok(new ProductResponseModel()
                {
                    ID = product.ID,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    PriceType = product.PriceType,
                    Logo = product.Logo,
                    CreationDate = product.CreationDate
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/product")]
        public async Task<IActionResult> Save([FromBody] ProductRequestModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                    return Unauthorized();

                if (request == null)
                    return BadRequest();

                if(request.ID.HasValue)
                {
                    var product = await _productService.Get(new Product() { ID = request.ID.Value, Email = Email });

                    if (product == null)
                        return NotFound();

                    product.Name = request.Name;
                    product.Description = request.Description;
                    product.Price = request.Price;
                    product.PriceType = request.PriceType;
                    product.Logo = request.Logo;

                    var isSuccess = await _productService.Update(product);

                    if (!isSuccess)
                        return BadRequest("UpdateFail");
                }
                else
                {
                    var productID = await _productService.Add(new Product()
                    {
                        Email = Email,
                        Name = request.Name,
                        Description = request.Description,
                        Price = request.Price,
                        PriceType = request.PriceType,
                        Logo = request.Logo,
                        CreationDate = DateTime.UtcNow
                    });
                }

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("/product/{productID}")]
        public async Task<IActionResult> Delete(Guid productID)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                    return Unauthorized();

                var product = await _productService.Get(new Product() { ID = productID, Email = Email });

                if (product == null)
                    return NotFound();

                var isSuccess = await _productService.Delete(new Product() { ID = productID, Email = Email });

                return isSuccess ? Ok() : BadRequest();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
