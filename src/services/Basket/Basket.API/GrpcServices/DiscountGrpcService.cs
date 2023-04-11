using Discount.GRPC.Protos;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        //Client
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoService;
        private readonly ILogger<DiscountGrpcService> _logger;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoService, ILogger<DiscountGrpcService> logger)
        {
            _discountProtoService = discountProtoService ?? throw new ArgumentNullException(nameof(discountProtoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            try
            {
                var discountRequest = new GetDiscountRequest { ProductName = productName };
                return await _discountProtoService.GetDiscountAsync(discountRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            
        }
    }
}
