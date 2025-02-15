namespace OrderManager.UI.Models
{
    public record CartItem
    {
        public int Quantity { get; set; }
        public ProductDTO Product { get; set; } = null!;
    }
}
