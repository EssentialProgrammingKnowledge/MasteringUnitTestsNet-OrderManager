using Microsoft.EntityFrameworkCore;
using OrderManager.API.Database;
using OrderManager.API.Models;

namespace OrderManager.API.Repositories
{
    internal sealed class CustomerRepository
        (
            OrderContext orderContext
        )
        : ICustomerRepository
    {
        public async Task<Customer> Add(Customer customer)
        {
            await orderContext.Customers.AddAsync(customer);
            await orderContext.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> Delete(Customer customer)
        {
            orderContext.Customers.Remove(customer);
            return await orderContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            return await orderContext.Customers.ToListAsync();
        }

        public async Task<Customer?> GetById(int id)
        {
            return await orderContext.Customers
                                     .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> Update(Customer customer)
        {
            orderContext.Customers.Update(customer);
            await orderContext.SaveChangesAsync();
            return customer;
        }
    }
}
