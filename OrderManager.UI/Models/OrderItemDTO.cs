namespace OrderManager.UI.Models
{
    public record OrderItemDTO(
        int ProductId,
        int Quantity)
    {
        public int ProductId { get; init; } = ProductId;
        public int Quantity { get; init; } = Quantity;
    }
}
