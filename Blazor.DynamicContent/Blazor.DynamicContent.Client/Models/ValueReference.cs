using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Blazor.DynamicContent.Client.Models
{
    public class ValueReference<T>
    {
        public string Id { get; set; }

        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (ValueChanged.HasDelegate)
                    ValueChanged.InvokeAsync(_value);
            }
        }

        public EventCallback<T> ValueChanged { get; set; }
    }
}