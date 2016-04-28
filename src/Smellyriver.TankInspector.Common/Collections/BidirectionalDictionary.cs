using System.Collections.Generic;
using System.Linq;

namespace Smellyriver.TankInspector.Common.Collections
{
    public class BidirectionalDictionary<TKey, TValue>: IDictionary<TKey, TValue>
    {

        private Dictionary<TKey, TValue> _forwardDictionary;
        private Dictionary<TValue, TKey> _reverseDicitonary;

        private Reversed _reversedDicitonary;
        public Reversed ReversedDictionary
        {
            get
            {
                if (_reversedDicitonary == null)
                    _reversedDicitonary = new Reversed(this);

                return _reversedDicitonary;
            }
        }

        public BidirectionalDictionary()
        {
            _forwardDictionary = new Dictionary<TKey, TValue>();
            _reverseDicitonary = new Dictionary<TValue, TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            _forwardDictionary.Add(key, value);
            _reverseDicitonary.Add(value, key);
        }

        public void Add(TValue value, TKey key)
        {
            _forwardDictionary.Add(key, value);
            _reverseDicitonary.Add(value, key);
        }

        public bool ContainsKey(TKey key)
        {
            return _forwardDictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _reverseDicitonary.ContainsKey(value);
        }

        public ICollection<TKey> Keys
        {
            get { return _forwardDictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            _reverseDicitonary.Remove(_forwardDictionary[key]);
            return _forwardDictionary.Remove(key);
        }

        public bool RemoveValue(TValue value)
        {
            _forwardDictionary.Remove( _reverseDicitonary[value]);
            return _reverseDicitonary.Remove(value); 
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _forwardDictionary.TryGetValue(key, out value);
        }

        public bool TryGetKey(TValue value, out TKey key)
        {
            return _reverseDicitonary.TryGetValue(value, out key);
        }

        public ICollection<TValue> Values
        {
            get { return _forwardDictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _forwardDictionary[key];
            }
            set
            {
                _forwardDictionary[key] = value;
                _reverseDicitonary[value] = key;
            }
        }

        public TKey this[TValue key]
        {
            get
            {
                return _reverseDicitonary[key];
            }
            set
            {
                _forwardDictionary[value] = key;
                _reverseDicitonary[key] = value;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            _forwardDictionary.Add(item.Key, item.Value);
            _reverseDicitonary.Add(item.Value, item.Key);
        }

        public void Clear()
        {
            _forwardDictionary.Clear();
            _reverseDicitonary.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _forwardDictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_forwardDictionary).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _forwardDictionary.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            _reverseDicitonary.Remove(item.Value);
            return _forwardDictionary.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _forwardDictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _forwardDictionary.GetEnumerator();
        }

        public class Reversed : IDictionary<TValue, TKey>
        {

            private BidirectionalDictionary<TKey, TValue> _forward;

            internal Reversed (BidirectionalDictionary<TKey, TValue> forward)
            {
                _forward = forward;
            }

            public void Add(TValue key, TKey value)
            {
                _forward._forwardDictionary.Add(value, key);
                _forward._reverseDicitonary.Add(key, value);
            }

            public bool ContainsKey(TValue key)
            {
                return _forward._reverseDicitonary.ContainsKey(key);
            }

            public ICollection<TValue> Keys
            {
                get { return _forward._reverseDicitonary.Keys; }
            }

            public bool Remove(TValue key)
            {
                _forward._forwardDictionary.Remove(_forward._reverseDicitonary[key]);
                return _forward._reverseDicitonary.Remove(key);
            }

            public bool TryGetValue(TValue key, out TKey value)
            {
                return _forward._reverseDicitonary.TryGetValue(key, out value);
            }

            public ICollection<TKey> Values
            {
                get { return _forward._reverseDicitonary.Values; }
            }

            public TKey this[TValue key]
            {
                get
                {
                    return _forward._reverseDicitonary[key];
                }
                set
                {
                    _forward._forwardDictionary[value] = key;
                    _forward._reverseDicitonary[key] = value;
                }
            }

            void ICollection<KeyValuePair<TValue, TKey>>.Add(KeyValuePair<TValue, TKey> item)
            {
                _forward._forwardDictionary.Add(item.Value, item.Key);
                _forward._reverseDicitonary.Add(item.Key, item.Value);
            }

            public void Clear()
            {
                _forward._forwardDictionary.Clear();
                _forward._reverseDicitonary.Clear();
            }

            bool ICollection<KeyValuePair<TValue, TKey>>.Contains(KeyValuePair<TValue, TKey> item)
            {
                return _forward._reverseDicitonary.ContainsKey(item.Key);
            }

            void ICollection<KeyValuePair<TValue, TKey>>.CopyTo(KeyValuePair<TValue, TKey>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<TValue, TKey>>)_forward._reverseDicitonary).CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _forward._reverseDicitonary.Count; }
            }

            bool ICollection<KeyValuePair<TValue, TKey>>.IsReadOnly
            {
                get { return false; }
            }

            bool ICollection<KeyValuePair<TValue, TKey>>.Remove(KeyValuePair<TValue, TKey> item)
            {
                _forward._forwardDictionary.Remove(item.Value);
                return _forward._reverseDicitonary.Remove(item.Key);
            }

            public IEnumerator<KeyValuePair<TValue, TKey>> GetEnumerator()
            {
                return _forward._reverseDicitonary.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _forward._reverseDicitonary.GetEnumerator();
            }
        }


    }
}
