using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record UpdateOrder : ICommand<Result<OrderDetailsDTO>>
    {
        public UpdateOrderDto OrderDto { get; set; }

        public UpdateOrder(UpdateOrderDto orderDto)
        {
            OrderDto = orderDto;
        }

        public class UpdateOrderHandler : ICommandHandler<UpdateOrder, Result<OrderDetailsDTO>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IProductRepository _productRepository;
            private readonly ICustomerRepository _customerRepository;

            public UpdateOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
            {
                _orderRepository = orderRepository;
                _productRepository = productRepository;
                _customerRepository = customerRepository;
            }

            public async Task<Result<OrderDetailsDTO>> Handle(UpdateOrder command, CancellationToken cancellationToken = default)
            {
                var dto = command.OrderDto;

                var validationResult = OrderValidator.ValidatePositions(dto.NewPositions);
                if (!validationResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(validationResult.ErrorMessage!);
                }

                var order = await _orderRepository.GetDetailsById(dto.Id);
                if (order is null)
                {
                    return Result<OrderDetailsDTO>.NotFoundResult(OrderErrorMessages.NotFound(dto.Id));
                }

                if (!OrderValidator.CanModifyOrDeleteOrder(order))
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(OrderErrorMessages.OrderMustBeNewToModify());
                }

                var customer = await _customerRepository.GetById(dto.CustomerId);
                if (customer is null)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(CustomerErrorMessages.NotFound(dto.CustomerId));
                }

                var productIds = dto.NewPositions.Select(i => i.ProductId).ToList();
                productIds.AddRange(order.OrderItems.Select(oi => oi.ProductId).Distinct());
                var products = await _productRepository.GetProductsByIds(productIds);
                var productsDict = products.ToDictionary(p => p.Id);
                var addResult = order.AddPositions(dto.NewPositions, productsDict);
                if (!addResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(addResult.ErrorMessage!);
                }

                var removeResult = order.RemovePositions(dto.DeletePostions, productsDict);
                if (!removeResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(removeResult.ErrorMessage!);
                }

                order.CustomerId = customer.Id;
                order.Customer = customer;
                await _orderRepository.Update(order);
                return Result<OrderDetailsDTO>.OkResult(order.AsDetailsDto());
            }
        }
    }
}
