using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManager.API.Database;
using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Models;
using OrderManager.API.Validations;

namespace OrderManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController
        (
            OrderContext orderContext
        )
        : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<CustomerDTO>> GetCustomers()
        {
            var customers = await orderContext.Customers
                    .Select(c => c.AsDto())
                    .ToListAsync();
            return customers;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            var customer = await orderContext.Customers.FindAsync(id);

            if (customer is null)
            {
                return NotFound(CustomerErrorMessages.NotFound(id));
            }

            return Ok(customer.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> AddCustomer(CustomerDTO dto)
        {
            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };
            orderContext.Customers.Add(customer);
            await orderContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer.AsDto());
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CustomerDTO>> UpdateCustomer(int id, CustomerDTO dto)
        {
            var customer = await orderContext.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
            if (customer is null)
            {
                return NotFound(CustomerErrorMessages.NotFound(id));
            }

            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.Email = dto.Email;
            orderContext.Customers.Update(customer);
            await orderContext.SaveChangesAsync();

            return Ok(customer.AsDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await orderContext.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
            if (customer is null)
            {
                return NotFound(CustomerErrorMessages.NotFound(id));
            }

            if (await orderContext.Orders.AnyAsync(o => o.CustomerId == id))
            {
                return BadRequest(CustomerErrorMessages.CannotDeleteCustomerWithOrders(id));
            }

            orderContext.Customers.Remove(customer);
            await orderContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
