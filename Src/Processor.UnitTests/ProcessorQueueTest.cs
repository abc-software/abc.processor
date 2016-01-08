using System;
using NUnit.Framework;
using NUnit.Mocks;

using Abc.Processor;

namespace Abc.Processor.UnitTests {

    [TestFixture] 
    public class ProcessorQueueTest {
        private ProcessorQueue queue;
        private DynamicMock processorMock1;
        private IProcessor processor1;
        private DynamicMock processorMock2;
        private IProcessor processor2;

        [SetUp]
        public void CreateIntegrationRequestQueue() {
            queue = new ProcessorQueue();

            processorMock1 = new DynamicMock(typeof(IProcessor));
            processorMock1.SetReturnValue("get_Name", "Processor1");
            processor1 = (IProcessor)processorMock1.MockInstance;

            processorMock2 = new DynamicMock(typeof(IProcessor));
            processorMock2.SetReturnValue("get_Name", "Processor2");
            processor2 = (IProcessor)processorMock2.MockInstance;
        }

        [Test]
        public void EnqueueTest() {

            queue.Enqueue(processor1);
            Assert.IsTrue(queue.HasItemOnQueue(processor1));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor1));
            Assert.IsTrue(queue.GetNextRequest(processor1));

            queue.Enqueue(processor2);
            Assert.IsTrue(queue.HasItemOnQueue(processor1));
            Assert.IsTrue(queue.HasItemOnQueue(processor2));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor1));
            Assert.IsTrue(queue.HasItemPendingOnQueue(processor2));
            Assert.IsTrue(queue.GetNextRequest(processor1));
            Assert.IsFalse(queue.GetNextRequest(processor2));

            queue.Enqueue(processor2);
            Assert.IsTrue(queue.HasItemOnQueue(processor1));
            Assert.IsTrue(queue.HasItemOnQueue(processor2));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor1));
            Assert.IsTrue(queue.HasItemPendingOnQueue(processor2));
            Assert.IsTrue(queue.GetNextRequest(processor1));
            Assert.IsFalse(queue.GetNextRequest(processor2));

            queue.Enqueue(processor1);
            Assert.IsTrue(queue.HasItemOnQueue(processor1));
            Assert.IsTrue(queue.HasItemOnQueue(processor2));
            Assert.IsTrue(queue.HasItemPendingOnQueue(processor1));
            Assert.IsTrue(queue.HasItemPendingOnQueue(processor2));
            Assert.IsTrue(queue.GetNextRequest(processor1));
            Assert.IsFalse(queue.GetNextRequest(processor2));
        }

        [Test]
        public void DequeueTest() {

            queue.Enqueue(processor1);
            queue.Enqueue(processor2);
            queue.Enqueue(processor1);

            IProcessor proc = queue.Dequeue();
            Assert.AreEqual(processor1.Name, proc.Name);     
            Assert.IsTrue(queue.HasItemOnQueue(processor1));
            Assert.IsTrue(queue.HasItemOnQueue(processor2));
            Assert.IsTrue(queue.HasItemPendingOnQueue(processor1));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor2));
            Assert.IsFalse(queue.GetNextRequest(processor1));
            Assert.IsTrue(queue.GetNextRequest(processor2));

            proc = queue.Dequeue();
            Assert.AreEqual(processor2.Name, proc.Name);
            Assert.IsTrue(queue.HasItemOnQueue(processor1));
            Assert.IsFalse(queue.HasItemOnQueue(processor2));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor1));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor2));
            Assert.IsTrue(queue.GetNextRequest(processor1));
            Assert.IsFalse(queue.GetNextRequest(processor2));

            proc = queue.Dequeue();
            Assert.AreEqual(processor1.Name, proc.Name);
            Assert.IsFalse(queue.GetNextRequest(processor1));
        }

        [Test]
        public void RemovePendingItemsTest() {
            queue.Enqueue(processor1);
            queue.Enqueue(processor2);
            queue.Enqueue(processor1);

            queue.RemovePendingItems(processor1);  
            Assert.IsTrue(queue.HasItemOnQueue(processor1));
            Assert.IsTrue(queue.HasItemOnQueue(processor2));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor1));
            Assert.IsTrue(queue.HasItemPendingOnQueue(processor2));
        }

        [Test]
        public void RemoveItemsTest() {
            queue.Enqueue(processor1);
            queue.Enqueue(processor2);
            queue.Enqueue(processor1);

            queue.RemoveItems(processor1);
            Assert.IsFalse(queue.HasItemOnQueue(processor1));
            Assert.IsTrue(queue.HasItemOnQueue(processor2));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor1));
            Assert.IsFalse(queue.HasItemPendingOnQueue(processor2));
        }

    }
}
