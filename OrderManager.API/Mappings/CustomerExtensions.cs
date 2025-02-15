using OrderManager.API.DTO;
using OrderManager.API.Models;

namespace OrderManager.API.Mappings
{
    public static class CustomerExtensions
    {
        public static CustomerDTO AsDto(this Customer customer)
        {
            return new CustomerDTO(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Email
            );
        }
    }
}
