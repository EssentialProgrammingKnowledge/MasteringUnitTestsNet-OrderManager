using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record DeleteOrder : ICommand<Result>
    {
        public int Id { get; set; }

        public DeleteOrder(int id)
        {
            Id = id;
        }

        public class DeleteOrderHandler : ICommandHandler<DeleteOrder, Result>
        {
            private readonly IOrderRepository _orderRepository;

            public DeleteOrderHandler(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository;
            }

            public async Task<Result> Handle(DeleteOrder command, CancellationToken cancellationToken = default)
            {
                var order = await _orderRepository.GetDetailsById(command.Id);
                if (order is null)
                {
                    return Result.NotFoundResult(OrderErrorMessages.NotFound(command.Id));
                }

                if (!OrderValidator.CanModifyOrDeleteOrder(order))
                {
                    return Result.BadRequestResult(OrderErrorMessages.OrderMustBeNewToModify());
                }

                var productDict = order.OrderItems.Select(oi => oi.Product).ToDictionary(p => p.Id);
                IncreaseProductsStock(order.OrderItems, productDict);
                var deleteResult = await _orderRepository.Delete(order);
                if (!deleteResult)
                {
                    return Result.NotFoundResult(OrderErrorMessages.NotFound(command.Id));
                }

                return Result.NoContentResult();
            }

            private void IncreaseProductsStock(IEnumerable<OrderItem> orderItems, Dictionary<int, Product> productsDict)
            {
                foreach (var orderItem in orderItems)
                {
                    if (!productsDict.TryGetValue(orderItem.ProductId, out var product))
                    {
                        continue;
                    }

                    product.IncreaseStock(orderItem.Quantity);
                }
            }
        }
    }
}
