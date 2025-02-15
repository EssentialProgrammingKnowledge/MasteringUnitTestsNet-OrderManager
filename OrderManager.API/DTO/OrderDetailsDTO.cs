using OrderManager.API.Models;

namespace OrderManager.API.DTO
{
    public record OrderDetailsDTO(
        int Id,
        string OrderNumber,
        decimal TotalPrice,
        OrderStatus OrderStatus,
        DateTime CreatedAt,
        CustomerDTO Customer,
        IEnumerable<OrderPositionDTO> Positions
    );
}
