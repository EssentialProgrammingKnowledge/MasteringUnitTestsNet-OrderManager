using OrderManager.API.Models;

namespace OrderManager.API.DTO
{
    public record ChangeOrderStatusDTO(
        int Id,
        OrderStatus OrderStatus
    );
}
