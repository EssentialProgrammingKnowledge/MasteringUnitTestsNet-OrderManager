using System.Text.Json.Serialization;

namespace OrderManager.API.DTO
{
    public record ErrorMessage(string Code, string Message, Dictionary<string, object>? Parameters = null)
    {
        public string Code { get; init; } = Code;
        public string Message { get; init; } = Message;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Parameters { get; init; } = Parameters;
    }
}
