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
            return new ErrorMessage("CUSTOMER_CANNOT_DELETE_WITH_ORDERS", $"Customer with id '{customerId}' cannot be deleted because of existing orders.",
                new Dictionary<string, object>
                {
                    { "Id", customerId }
                });
        }

        public static ErrorMessage FirstNameTooLong(int expectedLength, int currentLength)
        {
            return new ErrorMessage("CUSTOMER_FIRST_NAME_TOO_LONG", $"The customer has too long first name. Expected length: {expectedLength}, actual length: {currentLength}.",
                new Dictionary<string, object>
                {
                    { "ExpectedLength", expectedLength },
                    { "CurrentLength", currentLength }
                });
        }

        public static ErrorMessage FirstNameCannotBeEmpty(int customerId)
        {
            return new ErrorMessage("CUSTOMER_FIRST_NAME_CANNOT_BE_EMPTY", $"The customer with id '{customerId}' cannot have an empty first name.",
                new Dictionary<string, object>
                {
                    { "CustomerId", customerId }
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

        public static ErrorMessage LastNameCannotBeEmpty(int customerId)
        {
            return new ErrorMessage("CUSTOMER_FIRST_NAME_CANNOT_BE_EMPTY", $"The customer with id '{customerId}' cannot have an empty last name.",
                new Dictionary<string, object>
                {
                    { "CustomerId", customerId }
                });
        }

        public static ErrorMessage InvalidEmail(string email)
        {
            return new ErrorMessage("CUSTOMER_INVALID_EMAIL", $"The customer has invalid email '{email ?? string.Empty}'.",
                new Dictionary<string, object>
                {
                    { "Email", email ?? string.Empty }
                });
        }
    }
}
