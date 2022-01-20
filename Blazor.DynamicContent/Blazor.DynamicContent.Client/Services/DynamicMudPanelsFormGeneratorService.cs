using System.Dynamic;
using System.Linq.Expressions;
using System.Text.Json;
using Blazor.DynamicContent.Client.Models;
using Blazor.DynamicContent.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazor.DynamicContent.Client.Services
{
    public class DynamicMudPanelsFormGeneratorService
    {
        public RenderFragment RenderControls(Section[] sections, ExpandoObject data) => builder =>
        {
            var keyIndex = 0;
            builder.OpenComponent<MudBlazor.MudExpansionPanels>(keyIndex++);
            builder.AddAttribute(keyIndex++, "class", "mt-4");
            builder.AddAttribute(keyIndex++, nameof(MudBlazor.MudExpansionPanels.MultiExpansion), true);
            builder.AddAttribute(keyIndex++, nameof(MudBlazor.MudExpansionPanels.ChildContent), new RenderFragment((panelBuilder =>
            {
                foreach (var section in sections)
                {
                    var index = 1;
                    panelBuilder.OpenComponent<MudBlazor.MudExpansionPanel>(index++);
                    panelBuilder.AddAttribute(index++, nameof(MudBlazor.MudExpansionPanel.IsInitiallyExpanded), true);
                    panelBuilder.AddAttribute(index++, nameof(MudBlazor.MudExpansionPanel.TitleContent),
                        new RenderFragment(titleBuilder =>
                        {
                            var titleIndex = 0;
                            titleBuilder.OpenComponent<MudBlazor.MudText>(titleIndex++);
                            titleBuilder.AddAttribute(titleIndex++, nameof(MudBlazor.MudText.Typo), MudBlazor.Typo.h5);
                            titleBuilder.AddAttribute(titleIndex++, nameof(MudBlazor.MudText.ChildContent), new RenderFragment(titleContentBuilder =>
                            {
                                var contentIndex = 0;
                                titleContentBuilder.OpenElement(contentIndex++, "span");
                                titleContentBuilder.AddContent(contentIndex++, section.SectionName);
                                titleContentBuilder.CloseElement();
                            }));
                            titleBuilder.CloseComponent();
                        }));
                        panelBuilder.AddAttribute(index++, nameof(MudBlazor.MudExpansionPanel.ChildContent),
                        new RenderFragment(contentBuilder =>
                        {
                            foreach (var component in section.Components)
                            {
                                RenderInputComponent(data, component, contentBuilder);
                            }

                        }));
                    panelBuilder.CloseComponent();
                }
            })));
            builder.CloseComponent();
        };

        private void RenderInputComponent(ExpandoObject data, DynamicComponentModel component,
            RenderTreeBuilder builder)
        {
            var inputIndex = 0;
            switch (component.ComponentType)
            {
                case "textbox":
                case "customtextbox":
                    builder.OpenComponent(inputIndex++, typeof(MudBlazor.MudTextField<string>));
                    builder.AddAttribute(inputIndex++, nameof(MudBlazor.MudTextField<string>.Label), component.Label);
                    builder.AddAttribute(inputIndex++, nameof(MudBlazor.MudTextField<string>.Required), component.Required);
                    builder.AddAttribute(inputIndex++, nameof(MudBlazor.MudTextField<string>.ErrorText), component.ErrorText);
                    builder.AddAttribute(inputIndex++, nameof(MudBlazor.MudTextField<string>.Placeholder), component.EmptyText);
                    BindDataValue<string>(data, component.Id, builder, inputIndex);
                    builder.CloseComponent();
                    break;

                case "checkbox":
                    builder.OpenComponent(inputIndex++, typeof(MudBlazor.MudCheckBox<bool>));
                    builder.AddAttribute(inputIndex++, "Style", "margin-left: -1rem;");
                    builder.AddAttribute(inputIndex++, nameof(MudBlazor.MudCheckBox<bool>.Label), component.Label);
                    builder.AddAttribute(inputIndex++, nameof(MudBlazor.MudCheckBox<bool>.Color), MudBlazor.Color.Primary);
                    builder.AddAttribute(inputIndex++, nameof(MudBlazor.MudCheckBox<bool>.Required), component.Required);
                    BindDataValue<bool>(data, component.Id, builder, inputIndex, false, nameof(MudBlazor.MudCheckBox<bool>.Checked), nameof(MudBlazor.MudCheckBox<bool>.CheckedChanged));
                    builder.CloseComponent();
                    break;
                default:
                    builder.OpenElement(inputIndex++, "p");
                    builder.AddAttribute(inputIndex++, "style", "padding: 8px 12px  24px");
                    builder.AddContent(inputIndex++, $"Control type {component.ComponentType} is to be implemented :-)");
                    builder.CloseElement();
                    break;
            }
        }

        private void BindDataValue<T>(ExpandoObject data, string key, RenderTreeBuilder builder, int index, bool withValueExpression = false,
            string valuePropertyName = "Value", string valueChangedPropertyName = "ValueChanged")
        {
            var accessor = (IDictionary<string, object>)data;
            var value = GetValue<T>(accessor, key);
            accessor[key] = value;

            var valueChanged = RuntimeHelpers.TypeCheck(
                EventCallback.Factory.Create<T>(
                this, EventCallback.Factory.CreateInferred(this, __value =>
                {
                    accessor[key] = __value;
                    //Here can code added to change dependencies
                },
                GetValue<T>(accessor, key)
            )));

            builder.AddAttribute(index++, valuePropertyName, value);
            builder.AddAttribute(index++, valueChangedPropertyName, valueChanged);


            if (withValueExpression)
            {
                var formElementReference = new ValueReference<T>()
                {
                    Value = (T)value,
                    ValueChanged = valueChanged
                };
                var constantExpression = Expression.Constant(formElementReference, typeof(ValueReference<T>));
                var exp = Expression.Property(constantExpression, nameof(ValueReference<T>.Value));
                builder.AddAttribute(index++, "ValueExpression", Expression.Lambda<Func<T>>(exp));
            }
        }

        private T GetValue<T>(IDictionary<string, object> data, string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key] is JsonElement
                ? ((JsonElement)data[key]).ConvertToObject<T>()
                : (T)data[key];
            }

            return default(T);
        }
    }
}