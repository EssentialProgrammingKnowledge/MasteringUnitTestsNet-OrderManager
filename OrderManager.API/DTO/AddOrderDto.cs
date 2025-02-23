namespace OrderManager.API.DTO
{
    public record AddOrderDto
    {
        public int CustomerId { get; init; }
        public List<OrderItemDTO> Positions { get; init; } = [];

        public AddOrderDto(int customerId, List<OrderItemDTO> positions)
        {
            CustomerId = customerId;
            Positions = GetDistinctPositions(positions);
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
