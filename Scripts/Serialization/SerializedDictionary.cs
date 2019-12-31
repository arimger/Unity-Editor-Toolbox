//An idea orginally provided here - https://forum.unity.com/threads/finally-a-serializable-dictionary-for-unity-extracted-from-system-collections-generic.335797/, 2019

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO: needs reimplementation

namespace UnityEngine
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [SerializeField, HideInInspector]
        private int[] next;
        [SerializeField, HideInInspector]
        private int[] buckets;
        [SerializeField, HideInInspector]
        private int[] hashCodes;

        [SerializeField, HideInInspector]
        private int count;
        [SerializeField, HideInInspector]
        private int version;
        [SerializeField, HideInInspector]
        private int freeList;
        [SerializeField, HideInInspector]
        private int freeCount;

        [SerializeField, HideInInspector]
        private TKey[] keys;
        [SerializeField, HideInInspector]
        private TValue[] values;


        private readonly IEqualityComparer<TKey> comparer;


        public int Count
        {
            get { return count - freeCount; }
        }

        public TValue this[TKey key, TValue defaultValue]
        {
            get
            {
                var index = FindIndex(key);
                if (index >= 0)
                {
                    return values[index];
                }
                return defaultValue;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var index = FindIndex(key);
                if (index >= 0)
                {
                    return values[index];
                }

                throw new KeyNotFoundException(key.ToString());
            }

            set { Insert(key, value, false); }
        }


        public SerializedDictionary() : this(0, null)
        { }

        public SerializedDictionary(int capacity) : this(capacity, null)
        { }

        public SerializedDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
        { }

        public SerializedDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }
            
            Initialize(capacity);

            this.comparer = (comparer ?? EqualityComparer<TKey>.Default);
        }

        public SerializedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
        { }

        public SerializedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : this((dictionary != null) ? dictionary.Count : 0, comparer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            foreach (KeyValuePair<TKey, TValue> current in dictionary)
            {
                Add(current.Key, current.Value);
            }
        }


        public bool ContainsValue(TValue value)
        {
            if (value == null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (hashCodes[i] >= 0 && values[i] == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                var defaultComparer = EqualityComparer<TValue>.Default;
                for (int i = 0; i < count; i++)
                {
                    if (hashCodes[i] >= 0 && defaultComparer.Equals(values[i], value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return FindIndex(key) >= 0;
        }

        public void Clear()
        {
            if (count <= 0)
                return;

            for (int i = 0; i < buckets.Length; i++)
                buckets[i] = -1;

            Array.Clear(keys, 0, count);
            Array.Clear(values, 0, count);
            Array.Clear(hashCodes, 0, count);
            Array.Clear(next, 0, count);

            freeList = -1;
            count = 0;
            freeCount = 0;
            version++;
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            int[] bucketsCopy = new int[newSize];
            for (int i = 0; i < bucketsCopy.Length; i++)
                bucketsCopy[i] = -1;

            var keysCopy = new TKey[newSize];
            var valuesCopy = new TValue[newSize];
            var hashCodesCopy = new int[newSize];
            var nextCopy = new int[newSize];

            Array.Copy(values, 0, valuesCopy, 0, count);
            Array.Copy(keys, 0, keysCopy, 0, count);
            Array.Copy(hashCodes, 0, hashCodesCopy, 0, count);
            Array.Copy(next, 0, nextCopy, 0, count);

            if (forceNewHashCodes)
            {
                for (int i = 0; i < count; i++)
                {
                    if (hashCodesCopy[i] != -1)
                        hashCodesCopy[i] = (comparer.GetHashCode(keysCopy[i]) & 2147483647);
                }
            }

            for (int i = 0; i < count; i++)
            {
                int index = hashCodesCopy[i] % newSize;
                nextCopy[i] = bucketsCopy[index];
                bucketsCopy[index] = i;
            }

            buckets = bucketsCopy;
            keys = keysCopy;
            values = valuesCopy;
            hashCodes = hashCodesCopy;
            next = nextCopy;
        }

        private void Resize()
        {
            Resize(PrimeHelper.ExpandPrime(count), false);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int hash = comparer.GetHashCode(key) & 2147483647;
            int index = hash % buckets.Length;
            int num = -1;
            for (int i = buckets[index]; i >= 0; i = next[i])
            {
                if (hashCodes[i] == hash && comparer.Equals(keys[i], key))
                {
                    if (num < 0)
                        buckets[index] = next[i];
                    else
                        next[num] = next[i];

                    hashCodes[i] = -1;
                    next[i] = freeList;
                    keys[i] = default(TKey);
                    values[i] = default(TValue);
                    freeList = i;
                    freeCount++;
                    version++;
                    return true;
                }
                num = i;
            }
            return false;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (buckets == null)
                Initialize(0);

            int hash = comparer.GetHashCode(key) & 2147483647;
            int index = hash % buckets.Length;
            int num1 = 0;
            for (int i = buckets[index]; i >= 0; i = next[i])
            {
                if (hashCodes[i] == hash && comparer.Equals(keys[i], key))
                {
                    if (add)
                        throw new ArgumentException("Key already exists: " + key);

                    values[i] = value;
                    version++;
                    return;
                }
                num1++;
            }
            int num2;
            if (freeCount > 0)
            {
                num2 = freeList;
                freeList = next[num2];
                freeCount--;
            }
            else
            {
                if (count == keys.Length)
                {
                    Resize();
                    index = hash % buckets.Length;
                }
                num2 = count;
                count++;
            }
            hashCodes[num2] = hash;
            next[num2] = buckets[index];
            keys[num2] = key;
            values[num2] = value;
            buckets[index] = num2;
            version++;

            //if (num3 > 100 && HashHelpers.IsWellKnownEqualityComparer(comparer))
            //{
            //    comparer = (IEqualityComparer<TK>)HashHelpers.GetRandomizedEqualityComparer(comparer);
            //    Resize(entries.Length, true);
            //}
        }

        private void Initialize(int capacity)
        {
            int prime = PrimeHelper.GetPrime(capacity);

            buckets = new int[prime];
            for (int i = 0; i < buckets.Length; i++)
                buckets[i] = -1;

            keys = new TKey[prime];
            values = new TValue[prime];
            hashCodes = new int[prime];
            next = new int[prime];

            freeList = -1;
        }

        private int FindIndex(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (buckets != null)
            {
                int hash = comparer.GetHashCode(key) & 2147483647;
                for (int i = buckets[hash % buckets.Length]; i >= 0; i = next[i])
                {
                    if (hashCodes[i] == hash && comparer.Equals(keys[i], key))
                        return i;
                }
            }
            return -1;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = FindIndex(key);
            if (index >= 0)
            {
                value = values[index];
                return true;
            }
            value = default;
            return false;
        }

        public ICollection<TKey> Keys
        {
            get { return keys.Take(Count).ToArray(); }
        }

        public ICollection<TValue> Values
        {
            get { return values.Take(Count).ToArray(); }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            int index = FindIndex(item.Key);
            return index >= 0 &&
                EqualityComparer<TValue>.Default.Equals(values[index], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));

            if (array.Length - index < Count)
                throw new ArgumentException(string.Format("The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.", Count, array.Length));

            for (int i = 0; i < count; i++)
            {
                if (hashCodes[i] >= 0)
                    array[index++] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }


        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly SerializedDictionary<TKey, TValue> _Dictionary;
            private int _Version;
            private int _Index;
            private KeyValuePair<TKey, TValue> _Current;

            public KeyValuePair<TKey, TValue> Current
            {
                get { return _Current; }
            }

            internal Enumerator(SerializedDictionary<TKey, TValue> dictionary)
            {
                _Dictionary = dictionary;
                _Version = dictionary.version;
                _Current = default;
                _Index = 0;
            }

            public bool MoveNext()
            {
                if (_Version != _Dictionary.version)
                    throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary.version));

                while (_Index < _Dictionary.count)
                {
                    if (_Dictionary.hashCodes[_Index] >= 0)
                    {
                        _Current = new KeyValuePair<TKey, TValue>(_Dictionary.keys[_Index], _Dictionary.values[_Index]);
                        _Index++;
                        return true;
                    }
                    _Index++;
                }

                _Index = _Dictionary.count + 1;
                _Current = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                if (_Version != _Dictionary.version)
                    throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", _Version, _Dictionary.version));

                _Index = 0;
                _Current = default(KeyValuePair<TKey, TValue>);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            { }
        }


        private static class PrimeHelper
        {
            public static readonly int[] primes = new int[]
            {
                3,
                7,
                11,
                17,
                23,
                29,
                37,
                47,
                59,
                71,
                89,
                107,
                131,
                163,
                197,
                239,
                293,
                353,
                431,
                521,
                631,
                761,
                919,
                1103,
                1327,
                1597,
                1931,
                2333,
                2801,
                3371,
                4049,
                4861,
                5839,
                7013,
                8419,
                10103,
                12143,
                14591,
                17519,
                21023,
                25229,
                30293,
                36353,
                43627,
                52361,
                62851,
                75431,
                90523,
                108631,
                130363,
                156437,
                187751,
                225307,
                270371,
                324449,
                389357,
                467237,
                560689,
                672827,
                807403,
                968897,
                1162687,
                1395263,
                1674319,
                2009191,
                2411033,
                2893249,
                3471899,
                4166287,
                4999559,
                5999471,
                7199369
            };


            public static bool IsPrime(int candidate)
            {
                if ((candidate & 1) != 0)
                {
                    int num = (int)Math.Sqrt(candidate);
                    for (int i = 3; i <= num; i += 2)
                    {
                        if (candidate % i == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return candidate == 2;
            }


            public static int GetPrime(int min)
            {
                if (min < 0)
                {
                    throw new ArgumentException();
                }
                
                for (int i = 0; i < primes.Length; i++)
                {
                    int prime = primes[i];
                    if (prime >= min)
                    {
                        return prime;
                    }
                }

                for (int i = min | 1; i < 2147483647; i += 2)
                {
                    if (IsPrime(i) && (i - 1) % 101 != 0)
                    {
                        return i;
                    }
                }

                return min;
            }

            public static int ExpandPrime(int oldSize)
            {
                int num = 2 * oldSize;
                if (num > 2146435069 && 2146435069 > oldSize)
                {
                    return 2146435069;
                }
                return GetPrime(num);
            }
        }
    }
}