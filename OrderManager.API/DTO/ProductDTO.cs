using System.Text.Json.Serialization;

namespace OrderManager.API.DTO
{
    public record ProductDTO
    {
        public int Id { get; init; }
        public string ProductName { get; init; }
        public decimal Price { get; init; }
        public bool IsDigital { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ProductStockDTO? ProductStock { get; }

        public ProductDTO(
            int id,
            string productName,
            decimal price,
            bool isDigital,
            ProductStockDTO? productStock = null)
        {
            Id = id;
            ProductName = productName;
            Price = price;
            IsDigital = isDigital;
            ProductStock = productStock;
        }
    };

    public record ProductStockDTO(int Quantity);
}
