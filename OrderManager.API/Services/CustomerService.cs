using OrderManager.API.DTO;
using OrderManager.API.Mappings;
using OrderManager.API.Models;
using OrderManager.API.Repositories;
using OrderManager.API.Validations;

namespace OrderManager.API.Services
{
    internal sealed class CustomerService
        (
            ICustomerRepository customerRepository
        )
        : ICustomerService
    {
        public async Task<Result<CustomerDTO>> AddCustomer(CustomerDTO customerDto)
        {
            var result = Validate(customerDto);
            if (!result.Success)
            {
                return Result<CustomerDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email
            };
            await customerRepository.Add(customer);
            return Result<CustomerDTO>.CreatedResult(customer.AsDto());
        }

        public async Task<Result> DeleteCustomer(int id)
        {
            var customer = await customerRepository.GetById(id);
            if (customer is null)
            {
                return Result.NotFoundResult(CustomerErrorMessages.NotFound(id));
            }

            if (await customerRepository.HasAnyOrder(id))
            {
                return Result.BadRequestResult(CustomerErrorMessages.CannotDeleteCustomerWithOrders(id));
            }

            var result = await customerRepository.Delete(customer);
            return result ?
                Result.NoContentResult()
                : Result.NotFoundResult(CustomerErrorMessages.NotFound(id));
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomers()
        {
            return (await customerRepository.GetAll())
                            .Select(c => c.AsDto());
        }

        public async Task<Result<CustomerDTO>> GetCustomerById(int id)
        {
            var customer = await customerRepository.GetById(id);
            if (customer is null)
            {
                return Result<CustomerDTO>.NotFoundResult(CustomerErrorMessages.NotFound(id));
            }

            return Result<CustomerDTO>.OkResult(customer.AsDto());
        }

        public async Task<Result<CustomerDTO>> UpdateCustomer(CustomerDTO customerDto)
        {
            var result = Validate(customerDto);
            if (!result.Success)
            {
                return Result<CustomerDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var customer = await customerRepository.GetById(customerDto.Id);
            if (customer is null)
            {
                return Result<CustomerDTO>.NotFoundResult(CustomerErrorMessages.NotFound(customerDto.Id));
            }

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.Email = customerDto.Email;
            await customerRepository.Update(customer);
            return Result<CustomerDTO>.OkResult(customerDto);
        }

        private ValidationResult Validate(CustomerDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
            {
                return ValidationResult.FailureResult(CustomerErrorMessages.FirstNameCannotBeEmpty(dto.Id));
            }

            if (string.IsNullOrWhiteSpace(dto.LastName))
            {
                return ValidationResult.FailureResult(CustomerErrorMessages.LastNameCannotBeEmpty(dto.Id));
            }

            if (dto.FirstName.Length > 100)
            {
                return ValidationResult.FailureResult(CustomerErrorMessages.FirstNameTooLong(100, dto.FirstName.Length));
            }

            if (dto.LastName.Length > 100)
            {
               return ValidationResult.FailureResult(CustomerErrorMessages.LastNameTooLong(100, dto.LastName.Length));
            }

            if (!ValidateEmail(dto.Email))
            {
                return ValidationResult.FailureResult(CustomerErrorMessages.InvalidEmail(dto.Email));
            }

            return ValidationResult.SuccessResult();
        }

        private bool ValidateEmail(string? email)
        {
            // email should contain at least one character, at, one character, dot and one character 'a@a.a'
            if (string.IsNullOrWhiteSpace(email) || email.Length < 5)
            {
                return false;
            }

            // max 255
            if (email.Length > 255)
            {
                return false;
            }

            int atIndex = email.IndexOf('@');
            // should be exactly one '@'
            if (atIndex <= 0 || atIndex != email.LastIndexOf('@'))
            {
                return false;
            }

            string localPart = email[..atIndex];
            string domainPart = email[(atIndex + 1)..];

            if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domainPart))
            {
                return false;
            }

            // domain validation
            if (!domainPart.Contains('.') || domainPart.StartsWith('.') || domainPart.EndsWith('.'))
            {
                return false;
            }

            // double dot
            if (domainPart.Contains(".."))
            {
                return false;
            }

            // Validate after last dot one character
            int lastDotIndex = domainPart.LastIndexOf('.');
            if (lastDotIndex == domainPart.Length - 1)
            {
                return false;
            }

            return true;
        }
    }
}
