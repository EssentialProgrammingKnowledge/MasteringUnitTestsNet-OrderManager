using FluentValidation;

namespace OrderManager.UI.Models
{
    public record AddOrderPositionForm
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AddOrderPositionFormValidator : AbstractValidator<AddOrderPositionForm>
    {
        public AddOrderPositionFormValidator()
        {
            RuleFor(p => p.Quantity)
                .GreaterThan(0);

            RuleFor(e => e.ProductId)
               .GreaterThan(0)
               // custom validation message for forms
               // 0 means product is not choosen
               .WithMessage(ValidatorOptions.Global.LanguageManager.GetString("NotEmptyValidator"));

        }
    }
}
