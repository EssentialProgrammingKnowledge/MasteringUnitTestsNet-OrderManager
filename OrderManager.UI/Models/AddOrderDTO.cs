namespace OrderManager.UI.Models
{
    public record AddOrderDTO(int CustomerId, List<OrderItemDTO> Positions)
    {
        public int CustomerId { get; init; } = CustomerId;
        public List<OrderItemDTO> Positions { get; init; } = Positions;
    }
}
