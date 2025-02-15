using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record AddOrderPosition : ICommand<Result<OrderDetailsDTO>>
    {
        public int OrderId { get; }
        public OrderItemDTO NewPosition { get; }

        public AddOrderPosition(int orderId, OrderItemDTO newPosition)
        {
            OrderId = orderId;
            NewPosition = newPosition;
        }

        public class AddOrderPositionHandler : ICommandHandler<AddOrderPosition, Result<OrderDetailsDTO>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IProductRepository _productRepository;

            public AddOrderPositionHandler(IOrderRepository orderRepository, IProductRepository productRepository)
            {
                _orderRepository = orderRepository;
                _productRepository = productRepository;
            }

            public async Task<Result<OrderDetailsDTO>> Handle(AddOrderPosition command, CancellationToken cancellationToken = default)
            {
                var newPosition = command.NewPosition;
                var validationResult = OrderValidator.ValidatePosition(newPosition);
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

                var product = await _productRepository.GetById(newPosition.ProductId);
                if (product is null)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(ProductErrorMessages.NotFound(newPosition.ProductId));
                }

                var addPositionResult = order.AddPosition(newPosition, product);
                if (!addPositionResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(addPositionResult.ErrorMessage!);
                }

                await _orderRepository.Update(order);
                return Result<OrderDetailsDTO>.CreatedResult(order.AsDetailsDto());
            }
        }
    }
}
