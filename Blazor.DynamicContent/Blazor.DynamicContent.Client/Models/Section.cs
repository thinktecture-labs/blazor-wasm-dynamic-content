namespace Blazor.DynamicContent.Client.Models
{
    public class Section
    {
        public string Id { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public List<DynamicComponentModel> Components { get; set; } = new();
    }
}
