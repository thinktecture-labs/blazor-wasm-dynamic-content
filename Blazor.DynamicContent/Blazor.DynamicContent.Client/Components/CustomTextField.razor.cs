using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace Blazor.DynamicContent.Client.Components
{
    public partial class CustomTextField
    {
        [Parameter] public string Id { get; set; }
        [Parameter] public bool Required { get; set; }
        [CascadingParameter] EditContext CurrentEditContext { get; set; }

        private static string ErrorMessage = "Please enter a value";
        private ValidationMessageStore? _messageStore;
        private bool _showErrorMessage;

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(CustomTextField)} requires a cascading parameter " +
                    $"of type {nameof(EditContext)}. For example, you can use {nameof(CustomTextField)} inside " +
                    $"an {nameof(EditForm)}.");
            }

            _messageStore = new ValidationMessageStore(CurrentEditContext);

            // Add here custom validation for the current input textfield
            CurrentEditContext.OnValidationRequested += (s, e) =>
            {
                if (_messageStore != null && Required && String.IsNullOrWhiteSpace(Value))
                {
                    _showErrorMessage = true;
                    _messageStore.Add(CurrentEditContext.Field(Id), ErrorMessage);
                }
            };
            CurrentEditContext.OnFieldChanged += (s, e) =>
            {
                Console.WriteLine($"Field changed for {e.FieldIdentifier}");
                _messageStore.Clear(e.FieldIdentifier);
            };
        }

        private void OnValueChanged(object value)
        {
            _showErrorMessage = false;
            CurrentValue = value.ToString();
        }

        protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out string result, [NotNullWhen(false)] out string? validationErrorMessage)
        {
            result = value;
            validationErrorMessage = null;
            return !String.IsNullOrEmpty(value);
        }
    }
}