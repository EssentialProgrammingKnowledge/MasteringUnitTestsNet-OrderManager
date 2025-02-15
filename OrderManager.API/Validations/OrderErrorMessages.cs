using OrderManager.API.DTO;
using OrderManager.API.Models;

namespace OrderManager.API.Validations
{
    public static class OrderErrorMessages
    {
        public static ErrorMessage OrderMustContainAtLeastOneItem()
        {
            return new ErrorMessage("ORDER_MUST_CONTAIN_AT_LEAST_ONE_ITEM", "Order must contain at least one item.");
        }

        public static ErrorMessage OrderMustBeNewToModify()
        {
            return new ErrorMessage("ORDER_CANNOT_BE_MODIFIED_UNLESS_NEW", "You can only modify orders with the 'New' status.");
        }

        public static ErrorMessage OrderMustBeNewToDelete()
        {
            return new ErrorMessage("ORDER_CANNOT_BE_DELETED_UNLESS_NEW", "You can only delete orders with the 'New' status.");
        }

        public static ErrorMessage NotFound(int id)
        {
            return new ErrorMessage("ORDER_NOT_FOUND", $"Order with id '{id}' was not found.",
                new Dictionary<string, object>
                {
                    { "Id", id }
                });
        }

        public static ErrorMessage PositionNotFound(int orderId, int productId)
        {
            return new ErrorMessage("ORDER_POSITION_NOT_FOUND", $"Product with id '{productId}' is not part of Order with id '{orderId}'.",
                new Dictionary<string, object>
                {
                    { "OrderId", orderId },
                    { "ProductId", productId }
                });
        }

        public static ErrorMessage InvalidOrderStatus(OrderStatus orderStatus)
        {
            return new ErrorMessage("ORDER_INVALID_ORDER_STATUS", $"Invalid order status '{orderStatus}'.",
                new Dictionary<string, object>
                {
                    { "OrderStatus", orderStatus }
                });
        }

        public static ErrorMessage PositionMustBePresent()
        {
            return new ErrorMessage("ORDER_POSITION_MUST_BE_PRESENT", "The order position must be present in the order.");
        }

        public static ErrorMessage PositionQuantityMustBeGreaterThanZero(int productId, int quantity)
        {
            return new ErrorMessage("ORDER_POSITION_QUANTITY_MUST_BE_GREATER_THAN_ZERO", $"The quantity for product with id '{productId}' must be greater than zero, but it is '{quantity}'.",
                new Dictionary<string, object>
                {
                    { "ProductId", productId },
                    { "Quantity", quantity }
                });
        }

        public static ErrorMessage PositionsQuantityMustBeGreaterThanZero(IEnumerable<OrderItemDTO> dtos)
        {
            var invalidItems = dtos.Where(i => i.Quantity <= 0).ToList();
            var productDetails = invalidItems.Select(i => new { i.ProductId, i.Quantity }).ToList();

            var errorMessage = productDetails.Count > 1
                ? "Some products in the order have invalid quantities (<= 0)."
                : "One or more products in the order have invalid quantities (<= 0).";

            return new ErrorMessage("ORDER_POSITIONS_QUANTITY_MUST_BE_GREATER_THAN_ZERO", errorMessage,
                new Dictionary<string, object>
                {
                    { "InvalidItems", productDetails }
                });
        }

        public static ErrorMessage PositionsNotFound(int orderId, List<int> notFoundPositions)
        {
            return new ErrorMessage("ORDER_POSITIONS_NOT_FOUND", $"Products with IDs [{string.Join(", ", notFoundPositions)}] is not part of Order with id '{orderId}'.",
                new Dictionary<string, object>
                {
                    { "OrderId", orderId },
                    { "NotFoundPositions", notFoundPositions }
                });
        }

        public static ErrorMessage InvalidPositions()
        {
            return new ErrorMessage("ORDER_INVALID_POSITIONS_WHILE_ADD_OR_UPDATE", "During add or update, the order has invalid positions.");
        }

        public static ErrorMessage InvalidPosition()
        {
            return new ErrorMessage("ORDER_INVALID_POSITION_WHILE_ADD_OR_UPDATE", "During add or update, the order has invalid position.");
        }
    }
}
