namespace OrderManager.API.DTO
{
    public record UpdateOrderDto
    {
        public int Id { get; init; }
        public int CustomerId { get; init; }
        public List<OrderItemDTO> NewPositions { get; init; } = [];
        public List<int> DeletePostions { get; init; } = [];

        public UpdateOrderDto(int id, int customerId, List<OrderItemDTO> newPositions, List<int>? deletePostions = null)
        {
            Id = id;
            CustomerId = customerId;
            NewPositions = GetDistinctPositions(newPositions);
            DeletePostions = deletePostions ?? [];
        }

        private List<OrderItemDTO> GetDistinctPositions(IEnumerable<OrderItemDTO> orderItems)
        {
            if (orderItems is null)
            {
                return [];
            }

            return orderItems
                    .GroupBy(i => i.ProductId)
                    .Select(group => new OrderItemDTO(group.Key, group.Sum(i => i.Quantity)))
                    .ToList();
        }
    }
}
