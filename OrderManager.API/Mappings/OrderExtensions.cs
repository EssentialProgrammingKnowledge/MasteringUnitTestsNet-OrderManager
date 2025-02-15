using OrderManager.API.DTO;
using OrderManager.API.Models;

namespace OrderManager.API.Mappings
{
    public static class OrderExtensions
    {
        public static OrderDTO AsDto(this Order order)
        {
            return new OrderDTO(
                order.Id,
                order.OrderNumber,
                order.TotalPrice,
                order.OrderStatus,
                order.CreatedAt
            );
        }

        public static OrderDetailsDTO AsDetailsDto(this Order order)
        {
            return new OrderDetailsDTO(
                order.Id,
                order.OrderNumber,
                order.TotalPrice,
                order.OrderStatus,
                order.CreatedAt,
                order.Customer.AsDto(),
                order.OrderItems.Select(oi => new OrderPositionDTO(
                    oi.Id,
                    oi.Price,
                    oi.Quantity,
                    oi.Price * oi.Quantity,
                    oi.ProductId,
                    oi.Product?.ProductName ?? string.Empty
                ))
            );
        }
    }
}
