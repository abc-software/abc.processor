// ----------------------------------------------------------------------------
// <copyright file="ProcessorQueue.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Poreccosu izpildīšnas rinda
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is Queue")]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "No colllection name")]
    public class ProcessorQueue : Collection<IProcessor> {
        /// <summary>
        /// Inicializē jauno instanci <see cref="ProcessorQueue"/> class.
        /// </summary>
        public ProcessorQueue()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorQueue"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public ProcessorQueue(int capacity)
            : base(new List<IProcessor>(capacity)) {
        }

        /// <summary>
        /// Inicializē jauno instanci <see cref="ProcessorQueue"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="T:System.ArgumentNullException">collection is null.</exception>
        public ProcessorQueue(IEnumerable<IProcessor> collection)
            : base(new List<IProcessor>(collection)) {
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="T:System.Collections.Queue"></see>.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <exception cref="T:System.ArgumentNullException">The processor is <c>null</c>.</exception>
        public void Enqueue(IProcessor processor) {
            if (processor == null) {
                throw new ArgumentNullException("processor"); 
            }

            lock (this) {
                this.Add(processor);
            }
        }

        /// <summary>Removes and returns the object at the beginning of the <see cref="T:System.Collections.Queue"></see>.</summary>
        /// <returns>The object that is removed from the beginning of the <see cref="T:System.Collections.Queue"></see>.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Queue"></see> is empty. </exception>
        public IProcessor Dequeue() {
            if (this.Count == 0) {
                throw new InvalidOperationException(SR.InvalidOperationEmptyQueue);
            }

            IProcessor processor = null;
            lock (this) {
                processor = base[0];
                this.RemoveAt(0);
            }

            return processor; 
        }

        /// <summary>Returns the object at the beginning of the <see cref="T:System.Collections.Queue"></see> without removing it.</summary>
        /// <returns>The object at the beginning of the <see cref="T:System.Collections.Queue"></see>.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Queue"></see> is empty. </exception>
        public IProcessor Peek() {
            if (this.Count == 0) {
                throw new InvalidOperationException(SR.InvalidOperationEmptyQueue);
            }

            IProcessor processor = null;
            lock (this) {
                processor = base[0];
            }

            return processor; 
        }

        public bool GetNextRequest(IProcessor processor) {
            if (this.Count == 0) {
                return false;
            }

            IProcessor firstProcessor = null;
            lock (this) {
                firstProcessor = base[0];
            }

            return firstProcessor == processor;
        }

        public bool HasItemOnQueue(IProcessor processor) {
            return this.HasItemOnQueue(processor, false);
        }

        public bool HasItemPendingOnQueue(IProcessor processor) {
            return this.HasItemOnQueue(processor, true);
        }

        public void RemovePendingItems(IProcessor processor) {
            this.RemoveItems(processor, true);
        }

        public void RemoveItems(IProcessor processor) {
            this.RemoveItems(processor, false);
        }

        private bool HasItemOnQueue(IProcessor processor, bool pendingItemsOnly) {
            int startQueueIndex = pendingItemsOnly ? 1 : 0;
            lock (this) {
                for (int index = startQueueIndex; index < this.Count; index++) {
                    if (base[index] == processor) {
                        return true;
                    }
                }
            }

            return false;
        }

        private void RemoveItems(IProcessor processor, bool pendingItemsOnly) {
            // Note we are also potentially removing the item at index[0] as this method should
            // only be called when the thread performing the build has been stopped.
            int startQueueIndex = pendingItemsOnly ? 1 : 0;
            lock (this) {
                for (int index = this.Count - 1; index >= startQueueIndex; index--) {
                    if (base[index] == processor) {
                        this.RemoveAt(index);
                    }
                }
            }
        }
    }
}
