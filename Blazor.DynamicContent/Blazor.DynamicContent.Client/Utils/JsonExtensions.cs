using System.Text.Json;

namespace Blazor.DynamicContent.Client.Utils
{
    public static class JsonExtensions
    {
        public static T ConvertToObject<T>(this JsonElement element) => JsonSerializer.Deserialize<T>(element.GetRawText());
    }
}
