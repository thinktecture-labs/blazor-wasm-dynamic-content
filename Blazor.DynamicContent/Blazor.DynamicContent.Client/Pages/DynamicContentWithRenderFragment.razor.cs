using Blazor.DynamicContent.Client.Models;
using Blazor.DynamicContent.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Dynamic;
using System.Text.Json;

namespace Blazor.DynamicContent.Client.Pages
{
    public partial class DynamicContentWithRenderFragment
    {
        [Inject] public DynamicControlDataService DynamicControlDataService { get; set; }
        [Inject] public DynamicMudPanelsFormGeneratorService DynamicMudBlazorFormGeneratorService { get; set; }
        [Inject] public DynamicHtmlFormGeneratorService DynamicHtmlFormGeneratorService { get; set; }
        [Inject] public ISnackbar SnackbarProvider { get; set; }

        private bool _test = true;

        private MudForm _mudForm;
        private EditForm _form;
        private Section[] _sections = Array.Empty<Section>();
        private ExpandoObject model = new();

        protected override async Task OnInitializedAsync()
        {
            _sections = await DynamicControlDataService.LoadFormData();
            var dict = (IDictionary<string, Object>)model;
            var data = await DynamicControlDataService.LoadFormDataValues();
            if (data != null)
            {
                foreach (var keyValuePair in data)
                {
                    dict.Add(keyValuePair);
                }
            }
            else
            {
                Console.WriteLine("Data could not be parsed...");
            }
            await base.OnInitializedAsync();
        }

        private RenderFragment CreateFormWithMudBlazor()
        {
            return DynamicMudBlazorFormGeneratorService.RenderControls(_sections, model);
        }

        private RenderFragment CreateFormWithHtml()
        {
            return DynamicHtmlFormGeneratorService.RenderControls(_sections, model);
        }

        private async Task SubmitMudBlazorData()
        {
            await _mudForm.Validate();
            if (!_mudForm.IsValid)
            {
                Console.WriteLine("Form is not valid");
            }
            else
            {
                SnackbarProvider.Add("Form saved successfull.", Severity.Success);
                Console.WriteLine("Form submitted");
                Console.WriteLine($"Form value state: {JsonSerializer.Serialize(model)}");
            }
        }

        private async Task SubmitBlazorData()
        {
            if (!_form.EditContext.Validate())
            {
                Console.WriteLine("Form is not valid");
            }
            else
            {
                SnackbarProvider.Add("Form saved successfull.", Severity.Success);
                Console.WriteLine("Form submitted");
                Console.WriteLine($"Form value state: {JsonSerializer.Serialize(model)}");
            }
        }

    }
}