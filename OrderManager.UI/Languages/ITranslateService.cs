using OrderManager.UI.Models;

namespace OrderManager.UI.Languages
{
    public interface ITranslateService
    {
        string Translate(ErrorMessage errorMessage);
        string Translate(string translationKey, Dictionary<string, object>? parameters = null);
    }
}
