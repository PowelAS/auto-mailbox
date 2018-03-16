using System;
using System.Collections.Concurrent;

namespace AutoMailbox
{
    public class CircularQueue<T>
    {
        private readonly int _size;
        private readonly ConcurrentQueue<T> _q = new ConcurrentQueue<T>();

        public CircularQueue(int size)
        {
            _size = size;
        }

        public void Enqueue(T item)
        {
            _q.Enqueue(item);

            while (_q.Count > _size)
                TryDequeue(out T _);
        }

        public bool TryDequeue(out T result)
        {
            return _q.TryDequeue(out result);
        }

        public T[] ToArray()
        {
            return _q.ToArray();
        }

        public int Count => _q.Count;
    }
}