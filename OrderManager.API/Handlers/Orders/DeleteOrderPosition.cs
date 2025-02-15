using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record DeleteOrderPosition : ICommand<Result<OrderDetailsDTO>>
    {
        public int OrderId { get; }
        public int ProductId { get; }

        public DeleteOrderPosition(int orderId, int productId)
        {
            OrderId = orderId;
            ProductId = productId;
        }

        public class DeleteOrderPositionHandler : ICommandHandler<DeleteOrderPosition, Result<OrderDetailsDTO>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IProductRepository _productRepository;

            public DeleteOrderPositionHandler(IOrderRepository orderRepository, IProductRepository productRepository)
            {
                _orderRepository = orderRepository;
                _productRepository = productRepository;
            }

            public async Task<Result<OrderDetailsDTO>> Handle(DeleteOrderPosition command, CancellationToken cancellationToken = default)
            {
                var order = await _orderRepository.GetDetailsById(command.OrderId);
                if (order is null)
                {
                    return Result<OrderDetailsDTO>.NotFoundResult(OrderErrorMessages.NotFound(command.OrderId));
                }

                if (!OrderValidator.CanModifyOrDeleteOrder(order))
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(OrderErrorMessages.OrderMustBeNewToModify());
                }

                if (!order.OrderItems.Any(i => i.ProductId == command.ProductId))
                {
                    return Result<OrderDetailsDTO>.NotFoundResult(OrderErrorMessages.PositionNotFound(command.OrderId, command.ProductId));
                }

                var product = await _productRepository.GetById(command.ProductId);
                if (product is null)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(ProductErrorMessages.NotFound(command.ProductId));
                }

                var removePositionResult = order.RemovePosition(command.ProductId, product);
                if (!removePositionResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(removePositionResult.ErrorMessage!);
                }

                await _orderRepository.Update(order);
                return Result<OrderDetailsDTO>.OkResult(order.AsDetailsDto());
            }
        }
    }
}
