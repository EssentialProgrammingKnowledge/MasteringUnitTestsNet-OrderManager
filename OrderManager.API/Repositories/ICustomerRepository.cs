using OrderManager.API.Models;

namespace OrderManager.API.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetById(int id);
        Task<Customer> Add(Customer customer);
        Task<Customer> Update(Customer customer);
        Task<bool> Delete(Customer customer);
        Task<IEnumerable<Customer>> GetAll();
    }
}
