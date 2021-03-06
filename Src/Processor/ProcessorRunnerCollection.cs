// ----------------------------------------------------------------------------
// <copyright file="ProcessorRunnerCollection.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Processora izpildītaju kolekcija.
    /// </summary>
    public sealed class ProcessorRunnerCollection : IEnumerable<ProcessorRunner> {
        private Hashtable _processorRunners = new Hashtable();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorRunnerCollection"/> class.
        /// </summary>
        /// <param name="processors">Processoru kolekcija.</param>
        /// <param name="processingQueue">Processoru izpildīšana rinda.</param>
        public ProcessorRunnerCollection(ProcessorCollection processors, ProcessorQueue processingQueue) {
            if (processors == null) {
                throw new ArgumentNullException("processors");
            }

            if (processingQueue == null) {
                throw new ArgumentNullException("processingQueue");
            }

            foreach (IProcessor processor in processors) {
                ProcessorRunner runner = new ProcessorRunner(processor, processingQueue);
                _processorRunners.Add(processor.Name, runner);
            }
        }

        /// <summary>
        /// Atgriež kolekcijas ierakstu skaitu.
        /// </summary>
        /// <value>Ierakstu skaits.</value>
        public int Count {
            get {
                return _processorRunners.Count;
            }
        }

        /// <summary>
        /// Atgriež <see cref="IVIS.Infrastructure.Request.Processor.ProcessorRunner"/> ar attiecīgo processora izpildītaja nodaukumu.
        /// </summary>
        /// <param name="processorName">Procesora nosaukums.</param>
        /// <value>Processora izpildītājs.</value>
        public ProcessorRunner this[string processorName] {
            get {
                return _processorRunners[processorName] as ProcessorRunner;
            }
        }

        #region IEnumerable<ProcessorRunner> Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return _processorRunners.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<ProcessorRunner> IEnumerable<ProcessorRunner>.GetEnumerator() {
            return new ProcessorRunnerCollectionEnumerator(_processorRunners.Values.GetEnumerator());
        }

        #endregion

        private class ProcessorRunnerCollectionEnumerator : IEnumerator<ProcessorRunner> {
            private IEnumerator _enumerator;

            public ProcessorRunnerCollectionEnumerator(IEnumerator enumerator) {
                _enumerator = enumerator;
            }

            public ProcessorRunner Current {
                get { return (ProcessorRunner)_enumerator.Current; }
            }

            object IEnumerator.Current {
                get { return _enumerator.Current; }
            }

            public bool MoveNext() {
                return _enumerator.MoveNext();
            }

            public void Reset() {
                _enumerator.Reset();
            }

            public void Dispose() {
            }
        }
    }
}
