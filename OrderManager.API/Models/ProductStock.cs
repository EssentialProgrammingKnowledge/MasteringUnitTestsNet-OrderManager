namespace OrderManager.API.Models
{
    public class ProductStock
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = null!;
    }
}
