using Microsoft.EntityFrameworkCore;
using OrderManager.API.Database;
using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;

namespace OrderManager.API.Handlers.Orders
{
    public record GetAllOrders : ICommand<IEnumerable<OrderDTO>>
    {
        public class GetAllOrdersHandler : ICommandHandler<GetAllOrders, IEnumerable<OrderDTO>>
        {
            private readonly OrderContext _orderContext;

            public GetAllOrdersHandler(OrderContext orderContext)
            {
                _orderContext = orderContext;
            }

            public async Task<IEnumerable<OrderDTO>> Handle(GetAllOrders command, CancellationToken cancellationToken = default)
            {
                return await _orderContext.Orders
                                          .AsNoTracking()
                                          .Select(o => o.AsDto())
                                          .ToListAsync(cancellationToken);
            }
        }
    }
}
