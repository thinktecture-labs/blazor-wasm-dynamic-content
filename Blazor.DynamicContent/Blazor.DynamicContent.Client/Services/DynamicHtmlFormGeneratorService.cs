using System.Dynamic;
using System.Linq.Expressions;
using System.Text.Json;
using Blazor.DynamicContent.Client.Components;
using Blazor.DynamicContent.Client.Models;
using Blazor.DynamicContent.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazor.DynamicContent.Client.Services
{
    public class DynamicHtmlFormGeneratorService
    {
        public RenderFragment RenderControls(Section[] sections, ExpandoObject data) => builder =>
        {
            var keyIndex = 0;
            builder.OpenElement(keyIndex++, "div");
            builder.AddContent(keyIndex++, new RenderFragment((panelBuilder =>
            {
                foreach (var section in sections)
                {
                    var index = 1;
                    panelBuilder.OpenElement(index++, "div");
                    panelBuilder.AddAttribute(index++, "class" , "pa-6 mt-4 mud-elevation-2 rounded");
                    panelBuilder.OpenElement(index++, "h2");
                    panelBuilder.AddAttribute(index++, "class", "pb-6");
                    panelBuilder.AddContent(index++, section.SectionName);
                    panelBuilder.CloseElement();

                    panelBuilder.OpenElement(index++, "div");
                    panelBuilder.AddAttribute(index++, "class", "d-flex flex-column");
                    panelBuilder.AddAttribute(index++, "style", "gap: 1rem");
                    panelBuilder.AddContent(index++,
                    new RenderFragment(contentBuilder =>
                    {
                        foreach (var component in section.Components)
                        {
                            RenderInputComponent(data, component, contentBuilder);
                        }

                    }));
                    panelBuilder.CloseElement();
                    panelBuilder.CloseElement();
                }
            })));
            builder.CloseElement();
        };

        private void RenderInputComponent(ExpandoObject data, DynamicComponentModel component,
            RenderTreeBuilder builder)
        {
            var inputIndex = 0;
            switch (component.ComponentType)
            {
                case "textbox":
                    builder.OpenComponent(inputIndex++, typeof(CustomTextField));
                    builder.AddAttribute(inputIndex++, nameof(CustomTextField.Id), component.Id);
                    builder.AddAttribute(inputIndex++, nameof(CustomTextField.DisplayName), component.Label);
                    builder.AddAttribute(inputIndex++, nameof(CustomTextField.Required), component.Required);
                    BindDataValue<string>(data, component.Id, builder, inputIndex, true);
                    builder.CloseComponent();
                    break;

                case "checkbox":
                    builder.OpenElement(inputIndex++, "label");
                    builder.AddAttribute(inputIndex++, "class", "form-control");
                    builder.AddContent(inputIndex++, component.Label);
                    builder.OpenComponent(inputIndex++, typeof(InputCheckbox));
                    BindDataValue<bool>(data, component.Id, builder, inputIndex, true);
                    builder.CloseComponent();
                    builder.CloseElement();
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

            var formElementReference = new ValueReference<T>()
            {
                Id = key,
                Value = (T)value,
                ValueChanged = valueChanged
            };
            
            if (withValueExpression)
            {
                var constantExpression = Expression.Constant(formElementReference, typeof(ValueReference<T>));
                var memberExpression = Expression.Property(constantExpression, nameof(ValueReference<T>.Value));
                var expression = Expression.Lambda<Func<T>>(memberExpression);
                builder.AddAttribute(index++, "ValueExpression", expression);
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