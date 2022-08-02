using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Toolbox.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SearchableAttribute : PropertyAttribute
    {
        public List<string> Options { get; private set; } = new List<string>();
        
        private SearchableAttribute(){}

        public SearchableAttribute(List<string> options)
        {
            Options = options;
        }
    }
}