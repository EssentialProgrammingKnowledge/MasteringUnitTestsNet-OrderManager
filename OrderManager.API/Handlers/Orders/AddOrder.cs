using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record AddOrder : ICommand<Result<OrderDetailsDTO>>
    {
        public AddOrderDto OrderDto { get; }

        public AddOrder(AddOrderDto orderDto)
        {
            OrderDto = orderDto;
        }

        public class AddOrderHandler : ICommandHandler<AddOrder, Result<OrderDetailsDTO>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IProductRepository _productRepository;
            private readonly ICustomerRepository _customerRepository;

            public AddOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
            {
                _orderRepository = orderRepository;
                _productRepository = productRepository;
                _customerRepository = customerRepository;
            }

            public async Task<Result<OrderDetailsDTO>> Handle(AddOrder command, CancellationToken cancellationToken = default)
            {
                var dto = command.OrderDto;

                var validationResult = Validate(dto);
                if (!validationResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(validationResult.ErrorMessage!);
                }

                var customer = await _customerRepository.GetById(dto.CustomerId);
                if (customer is null)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(CustomerErrorMessages.NotFound(dto.CustomerId));
                }

                var productIds = dto.Positions.Select(i => i.ProductId).ToList();
                var products = await _productRepository.GetProductsByIds(productIds);
                var productsDict = products.ToDictionary(p => p.Id);
                var order = new Order
                {
                    OrderNumber = Guid.NewGuid().ToString().Substring(0, 10),
                    CreatedAt = DateTime.UtcNow,
                    OrderStatus = OrderStatus.New,
                    CustomerId = customer.Id,
                    Customer = customer
                };

                var addResult = order.AddPositions(dto.Positions, productsDict);
                if (!addResult.Success)
                {
                    return Result<OrderDetailsDTO>.BadRequestResult(addResult.ErrorMessage!);
                }

                await _orderRepository.Add(order);
                return Result<OrderDetailsDTO>.CreatedResult(order.AsDetailsDto());
            }

            private static ValidationResult Validate(AddOrderDto dto)
            {
                var positionsValidtionResult = OrderValidator.PositionShouldNotNullOrEmpty(dto.Positions);
                if (!positionsValidtionResult.Success)
                {
                    return positionsValidtionResult;
                }

                var validationResult = OrderValidator.ValidatePositions(dto.Positions);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                return ValidationResult.SuccessResult();
            }
        }
    }
}
