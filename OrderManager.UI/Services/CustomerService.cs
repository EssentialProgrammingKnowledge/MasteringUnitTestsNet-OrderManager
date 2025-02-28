using OrderManager.UI.Models;

namespace OrderManager.UI.Services
{
    public class CustomerService : ICustomerService
    {
        public Task<Result> Add(CustomerDTO customer)
        {
            throw new NotImplementedException();
        }

        public Task<Result> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CustomerDTO>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Result<CustomerDTO?>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result> Update(CustomerDTO customer)
        {
            throw new NotImplementedException();
        }
    }
}
