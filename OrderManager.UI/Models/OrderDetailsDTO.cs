namespace OrderManager.UI.Models
{
    public record OrderDetailsDTO(
        int Id,
        string OrderNumber,
        decimal TotalPrice,
        OrderStatus OrderStatus,
        DateTime CreatedAt,
        CustomerDTO Customer,
        List<OrderPositionDTO> Positions
    )
    {
        public int Id { get; set; } = Id;
        public string OrderNumber { get; set; } = OrderNumber;
        public decimal TotalPrice { get; set; } = TotalPrice;
        public OrderStatus OrderStatus { get; set; } = OrderStatus;
        public DateTime CreatedAt { get; set; } = CreatedAt;
        public CustomerDTO Customer { get; set; } = Customer;
        public List<OrderPositionDTO> Positions { get; set; } = Positions;
    }
}
