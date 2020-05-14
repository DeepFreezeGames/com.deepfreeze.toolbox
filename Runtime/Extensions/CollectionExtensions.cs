using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Runtime.Extensions
{
    public static class CollectionExtensions
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentException("Enumerable was null!");

            var count = enumerable.Count();
            if (count == 0)
                throw new IndexOutOfRangeException("Collection of length 0 provided!");

            var index = UnityEngine.Random.Range(0, count);
            return enumerable.ElementAt(index);
        }

        public static string PrettyString<T>(this IEnumerable<T> enumerable, Func<T, string> getter)
        {
            var builder = new StringBuilder();

            if (enumerable == null || !enumerable.Any())
            {
                return "EMPTY";
            }

            foreach (var part in enumerable)
            {
                builder.Append(getter(part));
                builder.Append(", ");
            }

            builder.Remove(builder.Length - 2, 2);

            return builder.ToString();
        }

        public static List<T> RemoveDuplicates<T>(this IEnumerable<T> input) where T : class
        {
            var output = new List<T>();
            foreach (var item in input)
            {
                if (output.Contains(item))
                {
                    continue;
                }
                
                output.Add(item);
            }

            return output;
        }

        /// <summary>
        /// Attempts to get a value from the dictionary.
        ///
        /// If the dictionary doesn't have the value, a new default version of the value is given.
        ///
        /// This is useful for loading serialized saves from a dictionary, because a new empty default instance
        /// will be automatically created for new saves.
        /// </summary>
        public static TVal DefaultGet<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key) where TVal : new()
        {
            return dict.DefaultGet(key, () => new TVal());
        }

        /// <summary>
        /// Attempts to get a value from the dictionary.
        ///
        /// If the dictionary doesn't have the value, a new default version of the value is given.
        ///
        /// This is useful for loading serialized saves from a dictionary, because a new empty default instance
        /// will be automatically created for new saves.
        /// </summary>
        public static TVal DefaultGet<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key, Func<TVal> newInstance)
        {
            if (dict.TryGetValue(key, out var value) && value != null)
            {
                return value;
            }

            var instance = newInstance();
            dict[key] = instance;
            return instance;
        }

        /// <summary>
        /// Attempts to get a value from the array.
        ///
        /// If the array doesn't have the index, or the value is null, a new default version of the value is given.
        ///
        /// If the requested index is above the maximum length of the list, the list will be extended up to
        /// the index.
        ///
        /// This is useful for loading serialized saves from an array, because a new empty default instance
        /// will be automatically created for new saves.
        /// </summary>
        public static T DefaultGet<T>(this List<T> list, int key) where T : class, new()
        {
            return list.DefaultGet(key, () => new T());
        }

        /// <summary>
        /// Attempts to get a value from the array.
        ///
        /// If the array doesn't have the index, or the value is null, a new default version of the value is given.
        ///
        /// If the requested index is above the maximum length of the list, the list will be extended up to
        /// the index.
        ///
        /// This is useful for loading serialized saves from an array, because a new empty default instance
        /// will be automatically created for new saves.
        /// </summary>
        public static T DefaultGet<T>(this List<T> list, int key, Func<T> newInstance) where T : class
        {
            if (key < 0)
            {
                throw new IndexOutOfRangeException("Key was " + key);
            }

            // Extend the list to fit the requested index
            while (key >= list.Count)
            {
                list.Add(null);
            }

            // Return the key if its not null
            var instance = list[key];
            if (instance != null)
            {
                return instance;
            }

            // Otherwise, put a new instance at the index
            instance = newInstance();
            list[key] = instance;
            return instance;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var index = UnityEngine.Random.Range(i, count);
                var temp = list[i];
                list[i] = list[index];
                list[index] = temp;
            }
        }

        public static void Swap<T>(this IList<T> list, int target, int source)
        {
            var temp = list[target];
            list[target] = list[source];
            list[source] = temp;
        }

        public static TVal VerboseGet<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey key, string description = "")
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException(
                $"Dictionary '{description}' of {typeof(TKey).Name}>{typeof(TVal).Name} didn't have key {key.ToString()}");
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return Distincter();

            IEnumerable<TSource> Distincter()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach (var element in source)
                {
                    if (knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }
    }
}