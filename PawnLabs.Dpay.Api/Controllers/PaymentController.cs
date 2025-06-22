using Microsoft.AspNetCore.Mvc;
using PawnLabs.Dpay.Api.Controllers.Base;
using PawnLabs.Dpay.Api.Model.Request;
using PawnLabs.Dpay.Api.Model.Response;
using PawnLabs.Dpay.Business.BackgroundService;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Api.Controllers
{
    public class PaymentController : BaseApiController
    {
        private IPaymentService _paymentService;
        private IProductService _productService;
        private IConfigurationService _configurationService;

        private StellarBackgroundService _stellarBackgroundService;


        public PaymentController(IPaymentService paymentService, IProductService productService, IConfigurationService configurationService, StellarBackgroundService stellarBackgroundService) 
        {
            _paymentService = paymentService;
            _productService = productService;
            _configurationService = configurationService;

            _stellarBackgroundService = stellarBackgroundService;
        }

        [HttpPost]
        [Route("/payment")]
        public async Task<IActionResult> Save([FromBody] PaymentRequestModel request)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || Application != EnumApplication.Modal)
                    return Unauthorized();

                if (request == null)
                    return BadRequest();

                await _paymentService.Add(new Payment()
                {
                    Email = Email,
                    ProductID = request.ProductID,
                    BuyerAddress = request.BuyerAddress,
                    WalletAddress = request.WalletAddress,
                    Price = request.Price,
                    PriceType = request.PriceType,
                    Date = DateTime.UtcNow
                });

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("/payment/qr")]
        public async Task<IActionResult> PayWithQR(Guid productID)
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || Application != EnumApplication.Modal)
                    return Unauthorized();

                var walletAddress = (string)((await _configurationService.GetByType(new Configuration() { Email = Email, Type = EnumConfigurationType.WalletAddress }))?.Value ?? string.Empty);

                await _stellarBackgroundService.StartJobAsync(productID, walletAddress);

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
