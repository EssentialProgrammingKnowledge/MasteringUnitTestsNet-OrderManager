using FluentValidation;

namespace OrderManager.UI.Models
{
    public record EditOrderPositionForm
    {
        public int Quantity { get; set; }
    }

    public class EditOrderPositionFormValidator : AbstractValidator<EditOrderPositionForm>
    {
        public EditOrderPositionFormValidator()
        {
            RuleFor(p => p.Quantity)
                .GreaterThan(0);
        }
    }
}
