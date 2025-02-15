using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record UpdateOrderPosition : ICommand<Result<OrderDetailsDTO>>
    {
        public int OrderId { get; }
        public OrderItemDTO UpdatePosition { get; }

        public UpdateOrderPosition(int orderId, OrderItemDTO updatePosition)
        {
            OrderId = orderId;
            UpdatePosition = updatePosition;
        }

        public class UpdateOrderPositionHandler : ICommandHandler<UpdateOrderPosition, Result<OrderDetailsDTO>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IProductRepository _productRepository;

            public UpdateOrderPositionHandler(IOrderRepository orderRepository, IProductRepository productRepository)
            {
                _orderRepository = orderRepository;
                _productRepository = productRepository;
            }

            public async Task<Result<OrderDetailsDTO>> Handle(UpdateOrderPosition command, CancellationToken cancellationToken)
            {
                var updatePosition = command.UpdatePosition;

                var validationResult = OrderValidator.ValidatePosition(updatePosition);
                if (!validationResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(validationResult.ErrorMessage!);
                }

                var order = await _orderRepository.GetDetailsById(command.OrderId);
                if (order is null)
                {
                    return Result<OrderDetailsDTO>.NotFoundResult(OrderErrorMessages.NotFound(command.OrderId));
                }

                if (!OrderValidator.CanModifyOrDeleteOrder(order))
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(OrderErrorMessages.OrderMustBeNewToModify());
                }

                if (!order.OrderItems.Any(i => i.ProductId == updatePosition.ProductId))
                {
                    return Result<OrderDetailsDTO>.NotFoundResult(OrderErrorMessages.PositionNotFound(command.OrderId, updatePosition.ProductId));
                }

                var product = await _productRepository.GetById(updatePosition.ProductId);
                if (product is null)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(ProductErrorMessages.NotFound(updatePosition.ProductId));
                }

                var modifyPositionResult = order.ModifyPosition(updatePosition, product);
                if (!modifyPositionResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(modifyPositionResult.ErrorMessage!);
                }

                await _orderRepository.Update(order);
                return Result<OrderDetailsDTO>.OkResult(order.AsDetailsDto());
            }
        }
    }
}
