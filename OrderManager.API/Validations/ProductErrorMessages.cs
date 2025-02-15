using OrderManager.API.DTO;

namespace OrderManager.API.Validations
{
    public static class ProductErrorMessages
    {
        public static ErrorMessage NotFound(int id)
        {
            return new ErrorMessage("PRODUCT_NOT_FOUND", $"Product with id '{id}' was not found.",
                new Dictionary<string, object>
                {
                    { "Id", id }
                });
        }

        public static ErrorMessage ProductsNotFound(IEnumerable<int> missingProductIds)
        {
            return new ErrorMessage("PRODUCTS_NOT_FOUND", $"Products with IDs [{string.Join(", ", missingProductIds)}] were not found.",
                new Dictionary<string, object>
                {
                    { "MissingProductIds", missingProductIds.ToList() }
                });
        }

        public static ErrorMessage ProductsNotAvailable(IEnumerable<int> notAvailableProductIds)
        {
            return new ErrorMessage("PRODUCTS_NOT_AVAILABLE", $"Products with IDs [{string.Join(", ", notAvailableProductIds)}] are not available.",
                new Dictionary<string, object>
                { 
                    { "NotAvailableProductIds", notAvailableProductIds.ToList() } 
                });
        }

        public static ErrorMessage ProductNotAvailable(int productId)
        {
            return new ErrorMessage("PRODUCT_NOT_AVAILABLE", $"Products with id '{productId}' is not available.",
                new Dictionary<string, object>
                {
                    { "Id", productId }
                });
        }

        public static ErrorMessage PriceMustBeGreaterThanZero(int productId)
        {
            return new ErrorMessage("PRODUCT_PRICE_MUST_BE_GREATER_THAN_ZERO", $"The price of the product with id '{productId}' must be greater than zero.",
                new Dictionary<string, object>
                {
                    { "ProductId", productId }
                });
        }

        public static ErrorMessage ProductNameCannotBeEmpty(int productId)
        {
            return new ErrorMessage("PRODUCT_NAME_CANNOT_BE_EMPTY", $"The product with id '{productId}' cannot have an empty name.",
                new Dictionary<string, object>
                {
                    { "ProductId", productId }
                });
        }

        public static ErrorMessage ProductNameTooLong(int expectedLength, int currentLength)
        {
            return new ErrorMessage("PRODUCT_NAME_TOO_LONG", $"The product has a name that is too long. Expected length: {expectedLength}, actual length: {currentLength}.",
                new Dictionary<string, object>
                {
                    { "ExpectedLength", expectedLength },
                    { "CurrentLength", currentLength }
                });
        }

        public static ErrorMessage ProductStockMustBePresent(int productId)
        {
            return new ErrorMessage("PRODUCT_STOCK_MUST_BE_PRESENT", $"The product with id '{productId}' must have stock information present.",
                new Dictionary<string, object>
                {
                    { "ProductId", productId }
                });
        }

        public static ErrorMessage ProductStockQuantityMustBeGreaterThanZero(int productId, int quantity)
        {
            return new ErrorMessage("PRODUCT_STOCK_QUANTITY_MUST_BE_GREATER_THAN_ZERO", $"The product with id '{productId}' has an invalid stock quantity '{quantity}'. It must be greater than zero.",
                new Dictionary<string, object>
                {
                    { "ProductId", productId },
                    { "Quantity", quantity }
                });
        }

        public static ErrorMessage CannotDeleteOrderedProduct(int productId)
        {
            return new ErrorMessage("PRODUCT_CANNOT_DELETE_ORDERED_PRODUCT", $"Product with ID '{productId}' cannot be deleted because it is part of an order.",
                new Dictionary<string, object>
                {
                    { "ProductId", productId }
                });
        }
    }
}
