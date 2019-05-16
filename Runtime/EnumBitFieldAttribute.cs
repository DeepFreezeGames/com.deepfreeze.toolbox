using System;
using UnityEngine;

namespace Toolbox.Runtime
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
