using System;
using System.Collections.Generic;

namespace GameCore.Utility.Pools
{
    public class LightPool<T>
    {
        private Stack<T> items;
        private Func<T> generator;
        private Action<T> onAlloc;
        private Action<T> onFree;
#if MEMORY_LEAK_DEBUG
		private Dictionary<T,object> allocated = new Dictionary<T, object>();
#endif
        private int capacity;

        public LightPool(Func<T> generator, int initialCapacity, Action<T> onAlloc = null, Action<T> onFree = null)
        {
#if MINI_MEMORY
            initialCapacity = 1;
#endif
            if (generator == null)
            {
                throw new ArgumentNullException("object generator");
            }

            this.items = new Stack<T>(initialCapacity);
            this.generator = generator;
            this.onAlloc = onAlloc;
            this.onFree = onFree;
            this.capacity = initialCapacity;

            for (int count = 0; count < initialCapacity; count++)
            {
                this.items.Push(this.generator());
            }
        }

        public void Resize(int newCapacity)
        {
            if (this.capacity >= newCapacity)
            {
                return;
            }
            for (int count = this.capacity; count < newCapacity; count++)
            {
                this.items.Push(this.generator());
            }

            this.capacity = newCapacity;
        }

        public T Alloc()
        {
            if (this.items.Count > 0)
            {
                T item = this.items.Pop();
                this.onAlloc?.Invoke(item);

#if MEMORY_LEAK_DEBUG
				allocated.Add(item, string.Intern(new System.Diagnostics.StackTrace(true).ToString()));
#endif

                return item;
            }
            else
            {
                T item = this.generator();
                this.onAlloc?.Invoke(item);
                this.capacity++;

#if MEMORY_LEAK_DEBUG
				if (allocated.Count % 1000 == 0)
					Trace.WriteLine($"LightPool new alloc {typeof(T)}");

				allocated.Add(item, string.Intern(new System.Diagnostics.StackTrace(true).ToString()));
#endif

                return item;
            }
        }

        public void Free(T item)
        {
            if (item == null)
            {
                return;
            }
#if MEMORY_LEAK_DEBUG
			var result = allocated.Remove(item);
			if(!result)
			{
				Trace.Assert(false);
			}
#endif
            this.onFree?.Invoke(item);
            this.items.Push(item);
        }

        public int GetAvailableCount()
        {
            return this.items.Count;
        }

        public int GetAllocedCount()
        {
            return this.capacity - this.items.Count;
        }
    }
}
