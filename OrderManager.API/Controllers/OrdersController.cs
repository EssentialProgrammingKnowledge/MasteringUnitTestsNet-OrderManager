using Microsoft.AspNetCore.Mvc;
using OrderManager.API.Dispatchers;
using OrderManager.API.DTO;
using OrderManager.API.Handlers.Orders;
using OrderManager.API.Mappings;

namespace OrderManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController
        (
            ICommandDispatcher commandDispatcher
        )
        : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            return await commandDispatcher.Send(new GetAllOrders());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrder(int id)
        {
            return (await commandDispatcher.Send(new GetOrderById(id))).ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetailsDTO>> AddOrder(AddOrderDto dto)
        {
            var result = await commandDispatcher.Send(new AddOrder(dto));
            return result.ToCreatedActionResult(this, nameof(GetOrder), new { result.Data?.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> UpdateOrder(int id, UpdateOrderDto order)
        {
            return (await commandDispatcher.Send(new UpdateOrder(order with { Id = id })))
                                      .ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            return (await commandDispatcher.Send(new DeleteOrder(id)))
                                      .ToActionResult();
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<OrderDTO>> ChangeStatus(int id, ChangeOrderStatusDTO dto)
        {
            return (await commandDispatcher.Send(new ChangeOrderStatus(dto with { Id = id })))
                                      .ToActionResult();
        }

        [HttpPost("{id:int}/positions")]
        public async Task<ActionResult<OrderDetailsDTO>> AddPosition(int id, OrderItemDTO dto)
        {
            return (await commandDispatcher.Send(new AddOrderPosition(id, dto)))
                                      .ToActionResult();

        }

        [HttpPatch("{id:int}/positions")]
        public async Task<ActionResult<OrderDetailsDTO>> UpdatePositionQuanity(int id, IEnumerable<OrderItemDTO> dtos)
        {
            return (await commandDispatcher.Send(new UpdateOrderPositions(id, dtos)))
                                      .ToActionResult();
        }

        [HttpPatch("{id:int}/positions/{productId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> UpdatePositionQuanity(int id, int productId, OrderItemDTO dto)
        {
            return (await commandDispatcher.Send(new UpdateOrderPosition(id, dto with { ProductId = productId })))
                                      .ToActionResult();
        }

        [HttpDelete("{id:int}/positions/{productId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> DeletePosition(int id, int productId)
        {
            return (await commandDispatcher.Send(new DeleteOrderPosition(id, productId)))
                                      .ToActionResult();
        }
    }
}
