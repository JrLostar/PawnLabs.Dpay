using PawnLabs.Dpay.Business.Service.Base.Impl;
using PawnLabs.Dpay.Core.Entity;

namespace PawnLabs.Dpay.Business.Service.Impl
{
    public class PaymentService : BaseService<Payment>, IPaymentService
    {
        public PaymentService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
