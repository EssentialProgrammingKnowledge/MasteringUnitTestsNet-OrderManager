using OrderManager.API.DTO;
using OrderManager.API.Models;
using OrderManager.API.Repositories;

namespace OrderManager.API.Services
{
    internal sealed class CustomerService
        (
            ICustomerRepository customerRepository
        )
        : ICustomerService
    {
        public Task<Result<CustomerDTO>> AddCustomer(CustomerDTO customerDto)
        {
            customerRepository.Add(new Customer());
            return Task.FromResult(Result<CustomerDTO>.OkResult(customerDto));
        }

        public Task<Result> DeleteCustomer(int id)
        {
            customerRepository.Delete(new Customer());
            return Task.FromResult(Result.NoContentResult());
        }

        public Task<IEnumerable<CustomerDTO>> GetAllCustomers()
        {
            customerRepository.GetAll();
            return Task.FromResult<IEnumerable<CustomerDTO>>([]);
        }

        public Task<Result<CustomerDTO>> GetCustomerById(int id)
        {
            customerRepository.GetById(id);
            return Task.FromResult(Result<CustomerDTO>.OkResult(new CustomerDTO(1, "First", "Last", "email")));
        }

        public Task<Result<CustomerDTO>> UpdateCustomer(CustomerDTO customerDto)
        {
            customerRepository.Update(new Customer());
            return Task.FromResult(Result<CustomerDTO>.OkResult(customerDto));
        }
    }
}
