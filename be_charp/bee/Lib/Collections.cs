using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Bee.Library
{
    public class ListCollection<T>
    {
        private List<T> list;

        public ListCollection()
        {
            list = new List<T>();
        }

        public ListCollection(int Size)
        {
            list = new List<T>(Size);
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            list.Add(item);
        }

        public void Add(ListCollection<T> items)
        {
            if (items == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            for(int i=0; i<items.Size(); i++)
            {
                Add(items.Get(i));
            }
        }

        public void Insert(int index, T item)
        {
            if (item == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            list.Insert(index, item);
        }

        public T Get(int index)
        {
            return list[index];
        }

        public T RemoveAt(int index)
        {
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }

        public T First()
        {
            if (Size() == 0)
            {
                throw new Exception("list is empty");
            }
            return list[0];
        }
        
        public T Last()
        {
            int size = Size();
            if (size == 0)
            {
                throw new Exception("list is empty");
            }
            return list[size-1];
        }

        public int Size()
        {
            return list.Count;
        }

        public bool IsNotEmpty()
        {
            return (list.Count > 0);
        }

        public bool IsEmpty()
        {
            return (list.Count == 0);
        }

        public void Clear()
        {
            list.Clear();
        }

        public T[] ToArray()
        {
            return list.ToArray();
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            for(int i=0; i<Size(); i++)
            {
                strBuilder.Append(Get(i));
                if(i < Size()-1)
                {
                    strBuilder.Append(", ");
                }
            }
            return strBuilder.ToString();
        }
    }

    public class MapCollection<K, V>
    {
        private Dictionary<K,V> map;

        public MapCollection()
        {
            map = new Dictionary<K, V>();
        }

        public void Put(K key, V value)
        {
            if (key == null)
            {
                throw new Exception("can not put null key reference to collection");
            }
            map[key] = value;
        }

        public V GetValue(K key)
        {
            return map[key];
        }

        public bool KeyExist(K key)
        {
            return map.ContainsKey(key);
        }

        public K[] GetKeys()
        {
            K[] keys = new K[map.Keys.Count];
            map.Keys.CopyTo(keys, 0);
            return keys;
        }

        public void Remove(K key)
        {
            map.Remove(key);
        }

        public int Size()
        {
            return map.Count;
        }

        public void Clear()
        {
            map.Clear();
        }
    }
}
