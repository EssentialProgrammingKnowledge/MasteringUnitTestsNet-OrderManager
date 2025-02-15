using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record ChangeOrderStatus : ICommand<Result<OrderDTO>>
    {
        public ChangeOrderStatusDTO Dto { get; }

        public ChangeOrderStatus(ChangeOrderStatusDTO dto)
        {
            Dto = dto;
        }

        public class ChangeOrderStatusHandler : ICommandHandler<ChangeOrderStatus, Result<OrderDTO>>
        {
            private readonly IOrderRepository _orderRepository;

            public ChangeOrderStatusHandler(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository;
            }

            public async Task<Result<OrderDTO>> Handle(ChangeOrderStatus command, CancellationToken cancellationToken = default)
            {
                var dto = command.Dto;
                var order = await _orderRepository.GetById(dto.Id);
                if (order is null)
                {
                    return Result<OrderDTO>.NotFoundResult(OrderErrorMessages.NotFound(dto.Id));
                }

                if (!Enum.IsDefined(dto.OrderStatus))
                {
                    return Result<OrderDTO>.BadRequestResult(OrderErrorMessages.InvalidOrderStatus(dto.OrderStatus));
                }

                order.OrderStatus = dto.OrderStatus;
                await _orderRepository.Update(order);
                return Result<OrderDTO>.OkResult(order.AsDto());
            }
        }
    }
}
