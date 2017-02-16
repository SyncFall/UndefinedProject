using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Bee.Library
{
    public class ListCollection<T>
    {
        protected List<T> list;

        public ListCollection()
        {
            list = new List<T>();
        }

        public ListCollection(int Size)
        {
            list = new List<T>(Size);
        }

        public ListCollection(ListCollection<T> Exist)
        {
            list = new List<T>();
            this.AddAll(Exist);
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            list.Add(item);
        }

        public void AddAll(ListCollection<T> Items)
        {
            if (Items == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            for(int i=0; i<Items.Size; i++)
            {
                Add(Items[i]);
            }
        }

        public void Remove(T Item)
        {
            list.Remove(Item);
        }

        public void RemoveAll(ListCollection<T> Items)
        {
            if (Items == null)
            {
                throw new Exception("can not remove null-reference from collection");
            }
            for (int i = 0; i < Items.Size; i++)
            {
                Remove(Items.Get(i));
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
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        public int IndexOf(T Item)
        {
            return list.IndexOf(Item);
        }

        public T First
        {
            get
            {
                if (list.Count == 0)
                {
                    throw new Exception("list is empty");
                }
                return list[0];
            }
        }
        
        public T Last
        {
            get
            {
                if (list.Count == 0)
                {
                    throw new Exception("list is empty");
                }
                return list[list.Count - 1];
            }
        }

        public int Size
        {
            get
            {
                return list.Count;
            }
        }

        public bool Contains(T Item)
        {
            for(int i=0; i<list.Count; i++)
            {
                if(list[i].Equals(Item))
                {
                    return true;
                }
            }
            return false;
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
            for(int i=0; i<Size; i++)
            {
                strBuilder.Append(Get(i));
                if(i < Size-1)
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

        public MapCollection(int Size)
        {
            map = new Dictionary<K, V>(Size);
        }

        public V this[K Key]
        {
            get
            {
                return map[Key];
            }
            set
            {
                map[Key] = value;
            }
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

        public bool IsEmpty()
        {
            return (map.Count == 0);
        }

        public bool IsNotEmpty()
        {
            return (map.Count > 0);
        }

        public void Clear()
        {
            map.Clear();
        }
    }

    public class SetCollection<T> : ListCollection<T>
    {
        public SetCollection() : base()
        { }

        public SetCollection(int Size) : base(Size)
        { }
       
        public void Add(T Item)
        {
            if (!list.Contains(Item))
            {
                list.Add(Item);
            }
        }
    }
}
