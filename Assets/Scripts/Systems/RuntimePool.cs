using System;
using System.Collections.Generic;

namespace Vanta.Systems
{
    public sealed class RuntimePool<T>
    {
        readonly Stack<T> available = new();
        readonly Func<T> factory;
        readonly int capacity;

        public int AvailableCount => available.Count;
        public RuntimePool(Func<T> factory, int capacity)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.capacity = Math.Max(0, capacity);
        }

        public T Rent() => available.Count > 0 ? available.Pop() : factory();

        public void Return(T item)
        {
            if (item == null || available.Count >= capacity) return;
            available.Push(item);
        }
    }
}