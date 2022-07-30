using System;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Runtime.Attributes
{
    public class EnumBitFieldAttribute : PropertyAttribute
    {
        public readonly Type EnumType;
        
        public EnumBitFieldAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}
