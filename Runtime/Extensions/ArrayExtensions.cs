using UnityEngine;

namespace Toolbox.Runtime.Extensions
{
    public static class ArrayExtensions
    {
        public static T RandomValue<T>(this T[] array)
        {
            var newIndex = Random.Range(0, array.Length);
            return array[newIndex];
        } 
    }
}