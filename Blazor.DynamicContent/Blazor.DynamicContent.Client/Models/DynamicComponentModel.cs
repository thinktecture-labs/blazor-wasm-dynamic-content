using System.Text.Json.Serialization;

namespace Blazor.DynamicContent.Client.Models
{
    public class DynamicComponentModel
    {
        public string Id { get; set; }
        public string ComponentType { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool Required { get; set; }
        public string EmptyText { get; set; } = string.Empty;
        public string ErrorText { get; set; } = string.Empty;
    }
}
