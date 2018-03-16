using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMailbox;
using Xunit;

namespace AutoMailboxTests
{
    public class CircularQueueTests
    {
        [Fact]
        public void Enqueue()
        {
            var q = new CircularQueue<int>(3);
            q.Enqueue(99);
            Assert.Equal(new[] { 99 }, q.ToArray());
        }

        [Fact]
        public void Dequeue()
        {
            var q = new CircularQueue<int>(3);
            q.Enqueue(99);

            bool success;
            int dequeued;

            success = q.TryDequeue(out dequeued);
            Assert.True(success);
            Assert.Equal(99, dequeued);

            success = q.TryDequeue(out dequeued);
            Assert.False(success);
        }

        [Fact]
        public void Overflow()
        {
            var q = new CircularQueue<int>(2);
            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);

            bool success;
            int dequeued;

            success = q.TryDequeue(out dequeued);
            Assert.True(success);
            Assert.Equal(2, dequeued);

            success = q.TryDequeue(out dequeued);
            Assert.True(success);
            Assert.Equal(3, dequeued);

            success = q.TryDequeue(out dequeued);
            Assert.False(success);
        }
    }
}
