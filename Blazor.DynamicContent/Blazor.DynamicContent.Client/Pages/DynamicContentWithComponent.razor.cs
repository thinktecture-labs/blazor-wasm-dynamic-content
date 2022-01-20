using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.DynamicContent.Client.Pages
{
    public partial class DynamicContentWithComponent
    {
        [Inject] public HttpClient? HttpClient { get; set; }

        private string namespaceComponents = "Blazor.DynamicContent.Client.Components.";
        private JSONComponentRoot? _root;
        protected async override Task OnInitializedAsync()
        {
            if (HttpClient is null)
            {
                return;
            }

            var currentComponents = await HttpClient.GetFromJsonAsync<JSONComponent[]>("./sample-data/dynamic-component-data.json");
            
            if (currentComponents != null)
            {
                foreach (var component in currentComponents)
                {
                    if (component.Parameters != null)
                    {
                        foreach (var parameter in component.Parameters)
                        {
                            var jsonElement = (JsonElement)parameter.Value;

                            switch (parameter.Key)
                            {
                                case "label":
                                case "value":
                                case "errorText":
                                case "emptyText":
                                    component.Parameters[parameter.Key] = jsonElement.GetString() ?? string.Empty;
                                    break;
                                case "checked":
                                case "required":
                                    component.Parameters[parameter.Key] = jsonElement.GetBoolean();
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                    component.ComponentType = Type.GetType($"{namespaceComponents}{component.Component}");
                }

                _root = new JSONComponentRoot { Components = currentComponents?.ToList() ?? new List<JSONComponent>() };
            }
        }

        private void SubmitData()
        {
            Console.WriteLine($"{JsonSerializer.Serialize(_root)}");
        }
    }

    public class JSONComponent
    {
        public int Id { get; set; }
        public string Component { get; set; } = string.Empty;
        [JsonIgnore]
        public Type? ComponentType { get; set; }
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    public class JSONComponentRoot
    {
        public List<JSONComponent> Components { get; set; }
    }
}