
namespace OrderManager.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsDigital { get; set; }
        public ProductStock? ProductStock { get; set; }
        public List<OrderItem> OrderItems { get; set; } = [];

        public bool HasStock()
        {
            return !IsDigital && (ProductStock?.Quantity ?? 0) > 0;
        }

        public void IncreaseStock(int quantity)
        {
            if (IsDigital)
            {
                return;
            }

            ProductStock!.Quantity += quantity;
        }

        public bool DecreaseStock(int quantity)
        {
            if (IsDigital)
            {
                return true;
            }

            if (!HasStock())
            {
                return false;
            }

            var quantityAfterUpdate = ProductStock!.Quantity - quantity;
            if (quantityAfterUpdate <= 0)
            {
                return false;
            }

            ProductStock!.Quantity -= quantity;
            return true;
        }
    }
}
