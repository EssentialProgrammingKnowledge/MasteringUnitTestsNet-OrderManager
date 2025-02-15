using OrderManager.UI.Models;
using System.Collections.Frozen;
using System.Text;

namespace OrderManager.UI.Languages
{
    public class TranslateService : ITranslateService
    {
        private readonly FrozenDictionary<string, string> _translations = new Dictionary<string, string>()
        {
            { "CUSTOMER_NOT_FOUND", "Klient o identyfikatorze '{Id}' nie został znaleziony." },
            { "CUSTOMER_CANNOT_DELETE_WITH_ORDERS", "Klient o identyfikatorze '{CustomerId}' nie może zostać usunięty, ponieważ posiada istniejące zamówienia." },
            { "CUSTOMER_FIRST_NAME_TOO_LONG", "Imię klienta jest zbyt długie. Oczekiwana długość: '{ExpectedLength}', aktualna długość: '{CurrentLength}'." },
            { "CUSTOMER_LAST_NAME_TOO_LONG", "Nazwisko klienta jest zbyt długie. Oczekiwana długość: '{ExpectedLength}', aktualna długość: '{CurrentLength}'." },
            { "CUSTOMER_NAME_TOO_LONG", "Email klienta jest zbyt długi. Oczekiwana długość: '{ExpectedLength}', aktualna długość: '{CurrentLength}'." },
            { "PRODUCT_NOT_FOUND", "Produkt o identyfikatorze '{Id}' nie został znaleziony." },
            { "PRODUCTS_NOT_FOUND", "Produkty o identyfikatorach '{MissingProductIds}' nie zostały znalezione." },
            { "PRODUCTS_NOT_AVAILABLE", "Produkty o identyfikatorach '{NotAvailableProductIds}' nie są dostępne." },
            { "PRODUCT_NOT_AVAILABLE", "Produkt o identyfikatorze '{ProductId}' nie jest dostępny." },
            { "PRODUCT_PRICE_MUST_BE_GREATER_THAN_ZERO", "Cena produktu o identyfikatorze '{ProductId}' musi być większa niż zero." },
            { "PRODUCT_NAME_CANNOT_BE_EMPTY", "Produkt o identyfikatorze '{ProductId}' nie może mieć pustej nazwy." },
            { "PRODUCT_NAME_TOO_LONG", "Nazwa produktu jest zbyt długa. Oczekiwana długość: '{ExpectedLength}', aktualna długość: '{CurrentLength}'." },
            { "PRODUCT_STOCK_MUST_BE_PRESENT", "Produkt o identyfikatorze '{ProductId}' musi mieć określoną ilość." },
            { "PRODUCT_STOCK_QUANTITY_MUST_BE_GREATER_THAN_ZERO", "Produkt o identyfikatorze '{ProductId}' ma niepoprawną ilość '{Quantity}'. Musi być większa niż zero." },
            { "PRODUCT_CANNOT_DELETE_ORDERED_PRODUCT", "Produkt o identyfikatorze '{ProductId}' nie może zostać usunięty, ponieważ jest częścią zamówienia." },
            { "ORDER_MUST_CONTAIN_AT_LEAST_ONE_ITEM", "Zamówienie musi zawierać co najmniej jeden produkt." },
            { "ORDER_CANNOT_BE_MODIFIED_UNLESS_NEW", "Można modyfikować tylko zamówienia ze statusem 'Nowe'." },
            { "ORDER_CANNOT_BE_DELETED_UNLESS_NEW", "Można usunąć tylko zamówienia ze statusem 'Nowe'." },
            { "ORDER_NOT_FOUND", "Zamówienie o identyfikatorze '{Id}' nie zostało znalezione." },
            { "ORDER_POSITION_NOT_FOUND", "Produkt o identyfikatorze '{ProductId}' nie jest częścią zamówienia o identyfikatorze '{OrderId}'." },
            { "ORDER_INVALID_ORDER_STATUS", "Nieprawidłowy status zamówienia '{OrderStatus}'." },
            { "ORDER_POSITION_MUST_BE_PRESENT", "Zamówienie musi posiadać pozycje." },
            { "ORDER_POSITION_QUANTITY_MUST_BE_GREATER_THAN_ZERO", "Ilość produktu '{Quantity}' o identyfikatorze '{ProductId}' musi być większa niż zero." },
            { "ORDER_POSITIONS_QUANTITY_MUST_BE_GREATER_THAN_ZERO", "Niektóre produkty w zamówieniu mają niepoprawne ilości (<= 0)." },
            { "ORDER_POSITIONS_NOT_FOUND", "Produkty o identyfikatorach '{NotFoundPositions}' nie są częścią zamówienia o identyfikatorze '{OrderId}'." },
            { "ORDER_INVALID_POSITIONS_WHILE_ADD_OR_UPDATE", "Podczas dodawania lub aktualizacji zamówienia wykryto nieprawidłowe pozycje – niektóre pozycje są puste lub brakuje wymaganych danych." },
            { "ORDER_INVALID_POSITION_WHILE_ADD_OR_UPDATE", "Podczas dodawania lub aktualizacji zamówienia wykryto nieprawidłową pozycję – brakuje wymaganych informacji." },
            { "GENERAL_ERROR", "Coś poszło nie tak, spróbuj ponownie później." },
            { "ORDER_STATUS_New", "Nowe" },
            { "ORDER_STATUS_InProgress", "W realizacji" },
            { "ORDER_STATUS_Completed", "Zakończone" }
        }.ToFrozenDictionary();

        public string Translate(ErrorMessage errorMessage)
        {
            if (errorMessage is null)
            {
                return string.Empty;
            }
            return Translate(errorMessage.Code, errorMessage.Parameters);
        }

        public string Translate(string translationKey, Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(translationKey))
            {
                return string.Empty;
            }

            _translations.TryGetValue(translationKey, out var translatedText);
            if (translatedText is null)
            {
                return translationKey;
            }

            return ReplaceParameters(translatedText, parameters);
        }

        private string ReplaceParameters(string translatedText, Dictionary<string, object>? parameters = null)
        {
            if (parameters is null || parameters.Count == 0)
            {
                return translatedText;
            }

            var translatedTextWithReplaceParams = new StringBuilder(translatedText);
            foreach (var param in parameters)
            {
                if (param.Key is null || param.Value is null)
                {
                    continue;
                }

                translatedTextWithReplaceParams.Replace($"{{{param.Key}}}", param.Value?.ToString() ?? "null");
            }

            return translatedTextWithReplaceParams.ToString();
        }
    }
}
