using OrderManager.UI.Models;

namespace OrderManager.UI.Services
{
    public interface IOrderService
    {
        Task<Result<OrderDetailsDTO?>> Add(AddOrderDTO dto);
        Task<Result<OrderDetailsDTO?>> GetById(int id);
        Task<Result> ChangeStatus(int id, OrderStatus orderStatus);
        Task<Result<List<OrderDTO>>> GetAll();
        Task<Result<OrderDetailsDTO>> Update(UpdateOrderDTO dto);
        Task<Result<OrderDetailsDTO>> UpdatePositions(int id, IEnumerable<OrderItemDTO> dtos);
        Task<Result> Delete(int id);
    }
}
