using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infraestructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Application.Exceptions;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderCommandHandler>    _logger;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<UpdateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderUpdate = await  _orderRepository.GetByIdAsync(request.Id);
            if (orderUpdate== null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }
            _mapper.Map(request, orderUpdate, typeof(UpdateOrderCommand), typeof(Order));

            await _orderRepository.UpdateAsync(orderUpdate);

            _logger.LogInformation($"Order {orderUpdate.Id} is successfully updated.");

            return Unit.Value;
        }
    }
}
