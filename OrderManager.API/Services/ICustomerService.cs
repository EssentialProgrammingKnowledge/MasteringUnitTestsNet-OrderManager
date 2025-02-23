using OrderManager.API.DTO;

namespace OrderManager.API.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllCustomers();
        Task<Result<CustomerDTO>> GetCustomerById(int id);
        Task<Result<CustomerDTO>> AddCustomer(CustomerDTO customerDto);
        Task<Result<CustomerDTO>> UpdateCustomer(CustomerDTO customerDto);
        Task<Result> DeleteCustomer(int id);
    }
}
