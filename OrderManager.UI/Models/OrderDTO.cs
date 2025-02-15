namespace OrderManager.UI.Models
{
    public record OrderDTO(
        int Id,
        string OrderNumber,
        decimal TotalPrice,
        OrderStatus OrderStatus,
        DateTime CreatedAt
    );
}
