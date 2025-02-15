namespace OrderManager.API.DTO
{
    public record OrderPositionDTO(int Id, decimal Price, int Quantity, decimal TotalPrice, int ProductId, string ProductName);
}
