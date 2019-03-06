using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;

namespace Core.Tests.Fakes
{
    public class FakeServiceCollection : IServiceCollection
    {
        private readonly List<ServiceDescriptor> _items = new List<ServiceDescriptor>();

        public ServiceDescriptor this[int index] { get => _items[index]; set => _items[index] = value; }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void Add(ServiceDescriptor item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(ServiceDescriptor item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            _items.Insert(index, item);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
