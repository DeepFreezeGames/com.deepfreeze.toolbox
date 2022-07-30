using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeepFreeze.Packages.Toolbox.Runtime
{
    public class ResourceReferenceAttribute : PropertyAttribute
    {
        public Type Type { get; private set; }

        public ResourceReferenceAttribute()
        {
            Type = typeof(Object);
        }

        public ResourceReferenceAttribute(Type type)
        {
            Type = type;
        }   
    }
}