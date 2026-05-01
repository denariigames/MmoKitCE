using System.Collections;
using System.Collections.Generic;

namespace NotifiableCollection
{
    public class NotifiableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public delegate void OnChangedDelegate(NotifiableDictionaryAction action, TKey key, TValue oldItem, TValue newItem);
        public delegate void OnChangedWithoutItemDelegate(NotifiableDictionaryAction action, TKey key);
        public event OnChangedDelegate DictionaryChanged;
        public event OnChangedWithoutItemDelegate DictionaryChangedWithoutItem;
        protected readonly Dictionary<TKey, TValue> _dictionary;
        private readonly object _lockObject = new object();

        private uint _version = 0;
        public uint Version
        {
            get
            {
                lock (_lockObject)
                {
                    return _version;
                }
            }
        }

        public NotifiableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public NotifiableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public NotifiableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            _dictionary = new Dictionary<TKey, TValue>(collection);
        }

        public NotifiableDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public NotifiableDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public NotifiableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public NotifiableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(collection, comparer);
        }

        public NotifiableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                lock (_lockObject)
                {
                    return _dictionary.Keys;
                }
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                lock (_lockObject)
                {
                    return _dictionary.Keys;
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                lock (_lockObject)
                {
                    return _dictionary.Values;
                }
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                lock (_lockObject)
                {
                    return _dictionary.Values;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_lockObject)
                {
                    return _dictionary.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public TValue this[TKey key]
        {
            get
            {
                lock (_lockObject)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_lockObject)
                {
                    if (_dictionary.TryGetValue(key, out TValue oldValue))
                    {
                        _dictionary[key] = value;
                        IncrementVersion();
                        InvokeNotifiableDictionaryAction(NotifiableDictionaryAction.Set, key, oldValue, value);
                    }
                    else
                    {
                        Add(key, value);
                    }
                }
            }
        }

        private void IncrementVersion()
        {
            _version++;
        }

        public void Add(TKey key, TValue value)
        {
            lock (_lockObject)
            {
                _dictionary.Add(key, value);
                IncrementVersion();
                InvokeNotifiableDictionaryAction(NotifiableDictionaryAction.Add, key, default, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            lock (_lockObject)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        public bool Remove(TKey key)
        {
            lock (_lockObject)
            {
                if (_dictionary.Remove(key, out TValue value))
                {
                    IncrementVersion();
                    InvokeNotifiableDictionaryAction(NotifiableDictionaryAction.Remove, key, value, default);
                    return true;
                }
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (_lockObject)
            {
                TKey key = item.Key;
                if (_dictionary.TryGetValue(key, out TValue value))
                {
                    if (EqualityComparer<TValue>.Default.Equals(value, item.Value))
                    {
                        if (_dictionary.Remove(key, out _))
                        {
                            IncrementVersion();
                            InvokeNotifiableDictionaryAction(NotifiableDictionaryAction.Remove, key, value, default);
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lockObject)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _dictionary.Clear();
                IncrementVersion();
                InvokeNotifiableDictionaryAction(NotifiableDictionaryAction.Clear, default, default, default);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (_lockObject)
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_lockObject)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_lockObject)
            {
                foreach (var item in _dictionary)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dirty(TKey key)
        {
            if (!TryGetValue(key, out TValue value))
                return;
            InvokeNotifiableDictionaryAction(NotifiableDictionaryAction.Dirty, key, value, value);
        }

        private void InvokeNotifiableDictionaryAction(NotifiableDictionaryAction action, TKey key, TValue oldItem, TValue newItem)
        {
            DictionaryChanged?.Invoke(action, key, oldItem, newItem);
            DictionaryChangedWithoutItem?.Invoke(action, key);
        }
    }
}