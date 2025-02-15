namespace OrderManager.UI.Models
{
    public record UpdateOrderDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public List<OrderItemDTO> NewPositions { get; set; } = [];
        public List<int> DeletePostions { get; set; } = [];
    }
}
