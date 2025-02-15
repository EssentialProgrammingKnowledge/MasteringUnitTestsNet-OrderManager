using Microsoft.EntityFrameworkCore;
using OrderManager.API.Database;
using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Validations;

namespace OrderManager.API.Handlers.Orders
{
    public record GetOrderById : ICommand<Result<OrderDetailsDTO>>
    {
        public int Id { get; set; }

        public GetOrderById(int id)
        {
            Id = id;
        }

        public class GetOrderByIdHandler : ICommandHandler<GetOrderById, Result<OrderDetailsDTO>>
        {
            private readonly OrderContext _orderContext;

            public GetOrderByIdHandler(OrderContext orderContext)
            {
                _orderContext = orderContext;
            }

            public async Task<Result<OrderDetailsDTO>> Handle(GetOrderById command, CancellationToken cancellationToken = default)
            {
                var order = await _orderContext.Orders
                                               .Include(o => o.OrderItems)
                                               .ThenInclude(oi => oi.Product)
                                               .Include(o => o.Customer)
                                               .FirstOrDefaultAsync(o => o.Id == command.Id, cancellationToken);
                return order is not null ?
                    Result<OrderDetailsDTO>.OkResult(order.AsDetailsDto()) :
                    Result<OrderDetailsDTO>.NotFoundResult(OrderErrorMessages.NotFound(command.Id));
            }
        }
    }
}
