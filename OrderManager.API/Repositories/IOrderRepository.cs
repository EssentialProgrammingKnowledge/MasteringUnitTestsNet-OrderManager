using OrderManager.API.Models;

namespace OrderManager.API.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetById(int id);
        Task<Order?> GetDetailsById(int id);
        Task<Order> Add(Order order);
        Task<Order> Update(Order order);
        Task<bool> Delete(Order order);
    }
}
