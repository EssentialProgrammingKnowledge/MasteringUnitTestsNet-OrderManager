using OrderManager.API.DTO;
using OrderManager.API.Models;

namespace OrderManager.API.Validations
{
    public static class OrderValidator
    {
        public static ValidationResult PositionShouldNotNullOrEmpty(IEnumerable<OrderItemDTO> positions)
        {
            if (positions is null || !positions.Any())
            {
                return ValidationResult.FailureResult(OrderErrorMessages.OrderMustContainAtLeastOneItem());
            }

            return ValidationResult.SuccessResult();
        }

        public static ValidationResult ValidatePosition(OrderItemDTO position)
        {
            if (position is null)
            {
                return ValidationResult.FailureResult(OrderErrorMessages.PositionMustBePresent());
            }

            if (position.Quantity <= 0)
            {
                return ValidationResult.FailureResult(OrderErrorMessages.PositionQuantityMustBeGreaterThanZero(position.ProductId, position.Quantity));
            }

            return ValidationResult.SuccessResult();
        }

        public static bool CanModifyOrDeleteOrder(Order order)
        {
            return order.OrderStatus == OrderStatus.New;
        }

        public static ValidationResult ValidatePositions(IEnumerable<OrderItemDTO> positions)
        {
            var invalidQuantityPositions = new List<OrderItemDTO>();
            foreach (var position in positions)
            {
                if (position is null)
                {
                    return ValidationResult.FailureResult(OrderErrorMessages.PositionMustBePresent());
                }

                if (position.Quantity <= 0)
                {
                    invalidQuantityPositions.Add(position);
                }
            }

            if (invalidQuantityPositions.Count > 0)
            {
                return ValidationResult.FailureResult(OrderErrorMessages.PositionsQuantityMustBeGreaterThanZero(invalidQuantityPositions));
            }

            return ValidationResult.SuccessResult();
        }
    }
}
