namespace OrderManager.UI.Models
{
    public record OrderPositionDTO(int Id, decimal Price, int Quantity, decimal TotalPrice, int ProductId, string ProductName)
    {
        public int Id { get; set; } = Id;
        public decimal Price { get; set; } = Price;
        public int Quantity { get; set; } = Quantity;
        public decimal TotalPrice { get; set; } = TotalPrice;
        public int ProductId { get; set; } = ProductId;
        public string ProductName { get; set; } = ProductName;
    }
}
