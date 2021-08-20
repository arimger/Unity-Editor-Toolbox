#if UNITY_2020_1_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    /// <summary>
    /// Overlay for the <see cref="Dictionary{TKey, TValue}"/> to allow serialization of the key/value pairs.
    /// </summary>
    [Serializable]
    public sealed class SerializedDictionary<TK, TV> : IDictionary<TK, TV>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct KeyValuePair
        {
            [SerializeField]
            private TK key;
            [SerializeField]
            private TV value;

            public KeyValuePair(TK key, TV value)
            {
                this.key = key;
                this.value = value;
            }

            public TK Key
            {
                get => key;
                set => key = value;
            }

            public TV Value
            {
                get => value;
                set => this.value = value;
            }
        }


        [SerializeField]
        private List<KeyValuePair> pairs = new List<KeyValuePair>();

        private readonly Dictionary<TK, int> indexByKey = new Dictionary<TK, int>();
        private readonly Dictionary<TK, TV> dictionary = new Dictionary<TK, TV>();

        [SerializeField, HideInInspector]
        private bool error;


        private void UpdateIndexes(int removedIndex)
        {
            for (var i = removedIndex; i < pairs.Count; i++)
            {
                var key = pairs[i].Key;
                indexByKey[key]--;
            }
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            dictionary.Clear();
            indexByKey.Clear();
            error = false;

            for (int i = 0; i < pairs.Count; i++)
            {
                var key = pairs[i].Key;
                if (key != null && !ContainsKey(key))
                {
                    dictionary.Add(key, pairs[i].Value);
                    indexByKey.Add(key, i);
                }
                else
                {
                    error = true;
                }
            }
        }

        public void Add(TK key, TV value)
        {
            pairs.Add(new KeyValuePair(key, value));
            dictionary.Add(key, value);
            indexByKey.Add(key, pairs.Count - 1);
        }

        public bool ContainsKey(TK key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(TK key)
        {
            if (dictionary.Remove(key))
            {
                var index = indexByKey[key];
                pairs.RemoveAt(index);
                UpdateIndexes(index);
                indexByKey.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(TK key, out TV value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            pairs.Clear();
            dictionary.Clear();
            indexByKey.Clear();
        }

        [Obsolete("Use BuildNativeDictionary instead.")]
        public Dictionary<TK, TV> BuiltNativeDictionary()
        {
            return new Dictionary<TK, TV>(dictionary);
        }

        public Dictionary<TK, TV> BuildNativeDictionary()
        {
            return new Dictionary<TK, TV>(dictionary);
        }

        void ICollection<KeyValuePair<TK, TV>>.Add(KeyValuePair<TK, TV> pair)
        {
            Add(pair.Key, pair.Value);
        }

        bool ICollection<KeyValuePair<TK, TV>>.Contains(KeyValuePair<TK, TV> pair)
        {
            if (dictionary.TryGetValue(pair.Key, out var value))
            {
                return EqualityComparer<TV>.Default.Equals(value, pair.Value);
            }
            else
            {
                return false;
            }
        }

        bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> pair)
        {
            if (dictionary.TryGetValue(pair.Key, out var value))
            {
                var isEqual = EqualityComparer<TV>.Default.Equals(value, pair.Value);
                if (isEqual)
                {
                    return Remove(pair.Key);
                }
            }

            return false;
        }

        void ICollection<KeyValuePair<TK, TV>>.CopyTo(KeyValuePair<TK, TV>[] array, int index)
        {
            ICollection collection = dictionary;
            collection.CopyTo(array, index);
        }

        IEnumerator<KeyValuePair<TK, TV>> IEnumerable<KeyValuePair<TK, TV>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }


        /// <summary>
        /// Indicates if there is a key collision in serialized pairs.
        /// Duplicated keys (pairs) won't be added to the final dictionary.
        /// This property is crucial for Editor-related functions.
        /// </summary>
        internal bool Error
        {
            get => error;
        }

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<TK> Keys => dictionary.Keys;

        public ICollection<TV> Values => dictionary.Values;

        public TV this[TK key]
        {
            get => dictionary[key];
            set
            {
                dictionary[key] = value;
                if (indexByKey.ContainsKey(key))
                {
                    var index = indexByKey[key];
                    pairs[index] = new KeyValuePair(key, value);
                }
                else
                {
                    pairs.Add(new KeyValuePair(key, value));
                    indexByKey.Add(key, pairs.Count - 1);
                }
            }
        }
    }
}
#endif