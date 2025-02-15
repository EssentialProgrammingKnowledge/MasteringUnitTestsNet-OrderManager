using OrderManager.API.DTO;

namespace OrderManager.API.Validations
{
    public static class CustomerErrorMessages
    {
        public static ErrorMessage NotFound(int id)
        {
            return new ErrorMessage("CUSTOMER_NOT_FOUND", $"Customer with id '{id}' was not found.",
                new Dictionary<string, object>
                {
                    { "Id", id }
                });
        }

        public static ErrorMessage CannotDeleteCustomerWithOrders(int customerId)
        {
            return new ErrorMessage("CUSTOMER_CANNOT_DELETE_WITH_ORDERS", $"Customer with id '{customerId}' cannot be deleted because of existing orders.");
        }

        public static ErrorMessage FirstNameTooLong(int productId, int expectedLength, int currentLength)
        {
            return new ErrorMessage("CUSTOMER_FIRST_NAME_TOO_LONG", $"The customer has too long first name. Expected length: {expectedLength}, actual length: {currentLength}.",
                new Dictionary<string, object>
                {
                    { "ExpectedLength", expectedLength },
                    { "CurrentLength", currentLength }
                });
        }

        public static ErrorMessage LastNameTooLong(int expectedLength, int currentLength)
        {
            return new ErrorMessage("CUSTOMER_LAST_NAME_TOO_LONG", $"The customer has too long last name. Expected length: {expectedLength}, actual length: {currentLength}.",
                new Dictionary<string, object>
                {
                    { "ExpectedLength", expectedLength },
                    { "CurrentLength", currentLength }
                });
        }

        public static ErrorMessage EmailTooLong(int expectedLength, int currentLength)
        {
            return new ErrorMessage("CUSTOMER_NAME_TOO_LONG", $"The customer has too long email. Expected length: {expectedLength}, actual length: {currentLength}.",
                new Dictionary<string, object>
                {
                    { "ExpectedLength", expectedLength },
                    { "CurrentLength", currentLength }
                });
        }
    }
}
