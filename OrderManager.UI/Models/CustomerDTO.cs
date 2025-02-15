using FluentValidation;

namespace OrderManager.UI.Models
{
    public record CustomerDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email {  get; set;  } = string.Empty;
    };

    public class CustomerValidator : AbstractValidator<CustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(c =>  c.FirstName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);
            
            RuleFor(c =>  c.LastName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(c =>  c.Email)
                .NotEmpty()
                .EmailAddress()
                .MinimumLength(3)
                .MaximumLength(250);
        }
    }
}
