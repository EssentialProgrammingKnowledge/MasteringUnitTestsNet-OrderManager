using Microsoft.EntityFrameworkCore;
using OrderManager.API.Database;
using OrderManager.API.Models;

namespace OrderManager.API.Repositories
{
    internal sealed class OrderRepository
        (
            OrderContext orderContext
        )
        : IOrderRepository
    {
        public async Task<Order> Add(Order order)
        {
            await orderContext.Orders.AddAsync(order);
            await orderContext.SaveChangesAsync();
            return order;
        }

        public async Task<bool> Delete(Order order)
        {
            orderContext.Orders.Remove(order);
            return await orderContext.SaveChangesAsync() > 0;
        }

        public async Task<Order?> GetById(int id)
        {
            return await orderContext.Orders
                                     .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetDetailsById(int id)
        {
            return await orderContext.Orders
                                     .Include(o => o.OrderItems)
                                     .ThenInclude(oi => oi.Product)
                                     .Include(c => c.Customer)
                                     .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> Update(Order order)
        {
            orderContext.Orders.Update(order);
            await orderContext.SaveChangesAsync();
            return order;
        }
    }
}
