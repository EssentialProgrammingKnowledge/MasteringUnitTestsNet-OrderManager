using Microsoft.AspNetCore.Mvc;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Services;

namespace OrderManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController
        (
            ICustomerService customerService
        )
        : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<CustomerDTO>> GetCustomers()
        {
            return await customerService.GetAllCustomers();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            return (await customerService.GetCustomerById(id))
                        .ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> AddCustomer(CustomerDTO dto)
        {
            return (await customerService.AddCustomer(dto))
                        .ToCreatedActionResult(this, nameof(GetCustomer), new { dto.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerDTO>> UpdateCustomer(int id, CustomerDTO dto)
        {
            return (await customerService.UpdateCustomer(dto with { Id = id }))
                        .ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            return (await customerService.DeleteCustomer(id))
                        .ToActionResult();
        }
    }
}
