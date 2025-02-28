using OrderManager.UI.Models;

namespace OrderManager.UI.Services
{
    public interface ICustomerService
    {
        Task<Result<List<CustomerDTO>>> GetAll();
        Task<Result<CustomerDTO?>> GetById(int id);
        Task<Result> Add(CustomerDTO customer);
        Task<Result> Update(CustomerDTO customer);
        Task<Result> Delete(int id);
    }
}
