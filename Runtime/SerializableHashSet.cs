using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeepFreeze.Packages.Toolbox.Runtime
{
    [Serializable]
    public class SerializableHashSet<TKey> : HashSet<TKey>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new();

        // Store the hashset as a list
        public void OnBeforeSerialize()
        {
            keys.Clear();
            foreach (var key in this)
            {
                keys.Add(key);
            }
        }

        // Store the dictionary from the pair of lists
        public void OnAfterDeserialize()
        {
            Clear();
            foreach (var t in keys)
            {
                Add(t);
            }
        }

        public List<TKey> ToList()
        {
            return keys;
        }
    }
}