using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;


namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IMapper mapper, IOrderRepository orderRepository, ILogger<DeleteOrderCommandHandler> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new NotImplementedException();

            var orderRepo=await _orderRepository.GetByIdAsync(request.Id);

           if (orderRepo == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }

            await _orderRepository.DeleteAsync(orderRepo);
            _logger.LogInformation($"Order {orderRepo.Id} is successfully deleted.");
        }
    }
}
