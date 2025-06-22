using Microsoft.AspNetCore.Mvc;
using PawnLabs.Dpay.Api.Controllers.Base;
using PawnLabs.Dpay.Api.Model.Response;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Core.Entity;

namespace PawnLabs.Dpay.Api.Controllers
{
    public class PaymentController : BaseApiController
    {
        private IPaymentService _paymentService;
        private IProductService _productService;

        public PaymentController(IPaymentService paymentService, IProductService productService) 
        {
            _paymentService = paymentService;
            _productService = productService;
        }

        [HttpGet]
        [Route("/payment/all")]
        public async Task<IActionResult> GetAll()
        {
            var response = new List<PaymentResponseModel>();

            try
            {
                if (string.IsNullOrEmpty(Email))
                    return Unauthorized();

                var payments = await _paymentService.GetAll(new Payment() { Email = Email });

                if(payments != null && payments?.Count > 0) 
                {
                    foreach(var payment in payments)
                    {
                        var product = await _productService.Get(new Product() { ID = payment.ProductID, Email = Email });

                        response.Add(new PaymentResponseModel()
                        {
                            ID = payment.ID,
                            ProductName = product?.Name ?? "NOT_FOUND",
                            ProductLogo = product?.Logo,
                            BuyerAddress = payment.BuyerAddress,
                            WalletAddress = payment.WalletAddress,
                            Price = payment.Price,
                            PriceType = payment.PriceType,
                            Date = payment.Date
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
    }
}
