using Microsoft.Extensions.Configuration;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Data.Repository.Base.Impl;

namespace PawnLabs.Dpay.Data.Repository.Impl
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
