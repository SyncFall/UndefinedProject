using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Be.Runtime.Types
{
    public class ListCollection<T>
    {
        public List<T> list;
        private int index;

        public ListCollection()
        {
            list = new List<T>();
            index = -1;
        }

        public virtual void Add(T item)
        {
            if (item == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            list.Add(item);
        }

        public virtual void Insert(int index, T item)
        {
            if (item == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            list.Insert(index, item);
        }

        public virtual T Get(int index)
        {
            return list[index];
        }


        public virtual T RemoveAt(int index)
        {
            T value = list[index];
            list.RemoveAt(index);
            return value;
        }

        public virtual void Rewind()
        {
            index = -1;
        }

        public virtual int Position()
        {
            return index;
        }

        public virtual T First()
        {
            int size = Size();
            if (size == 0)
            {
                throw new Exception("list is empty");
            }
            index = 0;
            return list[index];
        }

        public virtual bool IsFirst()
        {
            return (index <= 0);
        }

        public virtual T Next()
        {
            int size = Size();
            if (size == 0)
            {
                throw new Exception("list is empty");
            }
            else if (index >= size - 1)
            {
                throw new Exception("list position overflow");
            }
            index++;
            return list[index];
        }

        public virtual T Current()
        {
            int size = Size();
            if (size == 0)
            {
                throw new Exception("list is empty");
            }
            if (index == -1)
            {
                index = 0;
            }
            return list[index];
        }

        public virtual T Last()
        {
            int size = Size();
            if (size == 0)
            {
                throw new Exception("list is empty");
            }
            index = size - 1;
            return list[index];
        }

        public virtual bool IsLast()
        {
            return (index == Size() - 1);
        }

        public virtual bool IsNotEnd()
        {
            return (index < Size() - 1);
        }

        public virtual int Size()
        {
            return list.Count;
        }

        public virtual bool IsNotEmpty()
        {
            return (list.Count > 0);
        }

        public virtual bool IsEmpty()
        {
            return (list.Count == 0);
        }

        public virtual void Clear()
        {
            list.Clear();
        }

        public virtual T[] ToArray()
        {
            return list.ToArray();
        }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            for(int i=0; i<this.Size(); i++)
            {
                strBuilder.Append(this.Get(i).ToString());
                if(i < this.Size() - 1)
                {
                    strBuilder.Append(", ");
                }
            }
            return strBuilder.ToString();
        }

        public void Reverse()
        {
            list.Reverse();
        }
    }

    public class MapCollection<K, V>
    {
        private OrderedDictionary map;

        public MapCollection()
        {
            map = new OrderedDictionary();
        }

        public void Add(K key, V value)
        {
            if (key == null || value == null)
            {
                throw new Exception("can not add null-reference to collection");
            }
            map[key] = value;
        }

        public V GetValue(K key)
        {
            return (V)map[key];
        }

        public bool KeyExist(K key)
        {
            return map.Contains(key);
        }

        public V GetValue(int index)
        {
            return (V)map[index];
        }

        public K[] GetKeys()
        {
            K[] keys = new K[map.Keys.Count];
            map.Keys.CopyTo(keys, 0);
            return (K[])keys;
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
