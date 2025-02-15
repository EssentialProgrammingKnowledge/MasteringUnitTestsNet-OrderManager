using OrderManager.API.Models;

namespace OrderManager.API.DTO
{
    public record OrderDTO(
        int Id,
        string OrderNumber,
        decimal TotalPrice,
        OrderStatus OrderStatus,
        DateTime CreatedAt
    );
}
