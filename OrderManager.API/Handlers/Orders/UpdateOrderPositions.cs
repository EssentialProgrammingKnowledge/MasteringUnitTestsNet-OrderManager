using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record UpdateOrderPositions : ICommand<Result<OrderDetailsDTO>>
    {
        public int OrderId { get; }
        public IEnumerable<OrderItemDTO> UpdatePositions { get; } = [];

        public UpdateOrderPositions(int orderId, IEnumerable<OrderItemDTO> updatePositions)
        {
            OrderId = orderId;
            UpdatePositions = updatePositions;
        }

        public class UpdateOrderPositionsHandler : ICommandHandler<UpdateOrderPositions, Result<OrderDetailsDTO>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IProductRepository _productRepository;

            public UpdateOrderPositionsHandler(IOrderRepository orderRepository, IProductRepository productRepository)
            {
                _orderRepository = orderRepository;
                _productRepository = productRepository;
            }

            public async Task<Result<OrderDetailsDTO>> Handle(UpdateOrderPositions command, CancellationToken cancellationToken = default)
            {
                var validationResult = Validate(command);
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

                var missingItems = FindMissingProductsInOrder(command.UpdatePositions, order);
                if (missingItems.Count > 0)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(OrderErrorMessages.PositionsNotFound(command.OrderId, missingItems));
                }

                var productIds = command.UpdatePositions.Select(i => i.ProductId).ToList();
                var products = await _productRepository.GetProductsByIds(productIds);
                var productsDict = products.ToDictionary(p => p.Id);
                var updateResult = order.ModifyPostions(command.UpdatePositions, productsDict);
                if (!updateResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(updateResult.ErrorMessage!);
                }

                await _orderRepository.Update(order);
                return Result<OrderDetailsDTO>.OkResult(order.AsDetailsDto());
            }

            private List<int> FindMissingProductsInOrder(IEnumerable<OrderItemDTO> updatePositions, Order order)
            {
                var updateProductIds = new HashSet<int>(updatePositions.Select(u => u.ProductId));
                var orderProductIds = new HashSet<int>(order.OrderItems.Select(i => i.ProductId));

                return updateProductIds
                    .Where(id => !orderProductIds.Contains(id))
                    .ToList();
            }

            private ValidationResult Validate(UpdateOrderPositions dto)
            {
                var positionsResult = OrderValidator.PositionShouldNotNullOrEmpty(dto.UpdatePositions);
                if (!positionsResult.Success)
                {
                    return positionsResult;
                }

                var validationResult = OrderValidator.ValidatePositions(dto.UpdatePositions);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                return ValidationResult.SuccessResult();
            }
        }
    }
}
