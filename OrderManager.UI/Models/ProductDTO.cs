using FluentValidation;

namespace OrderManager.UI.Models
{
    public record ProductDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsDigital { get; set; }
        public ProductStockDTO? ProductStock { get; set; }
    }

    public record ProductStockDTO(int Quantity)
    {
        public int Quantity { get; set; } = Quantity;
    }

    public class ProductValidator : AbstractValidator<ProductDTO>
    {
        public ProductValidator()
        {
            RuleFor(p => p.ProductName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(200);

            RuleFor(p => p.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.ProductStock)
                .NotNull().When(p => !p.IsDigital);

            RuleFor(p => p.ProductStock!.Quantity)
                .GreaterThanOrEqualTo(0)
                .When(p => !p.IsDigital && p.ProductStock is not null);
        }
    }
}
