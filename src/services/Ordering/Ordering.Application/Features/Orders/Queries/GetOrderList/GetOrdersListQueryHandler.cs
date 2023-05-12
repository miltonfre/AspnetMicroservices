using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;


namespace Ordering.Application.Features.Orders.Queries.GetOrderList
{
    public class GetOrdersListQueryHandler : IRequestHandler<GetOrderListQuery, List<OrdersVm>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetOrdersListQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        async Task<List<OrdersVm>> IRequestHandler<GetOrderListQuery, List<OrdersVm>>.Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            var orderList=await _orderRepository.GetOrdersByUserName(request.UserName);
           return  _mapper.Map<List<OrdersVm>>(orderList);
        }


    }
}
