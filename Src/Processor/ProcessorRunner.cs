// ----------------------------------------------------------------------------
// <copyright file="ProcessorRunner.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// Processora izpildītais.
    /// </summary>
    public class ProcessorRunner : IDisposable {
        private readonly Triggers.ITrigger _trigger;
        private readonly IProcessor _processor;
        private readonly ProcessorQueue _processingQueue;
        private Thread _thread;
        private ProcessorState _state = ProcessorState.Stopped;

        /// <summary>
        /// Initializē jano klases <see cref="ProcessorRunner"/> instanci.
        /// </summary>
        /// <param name="processor">Processors.</param>
        /// <param name="processingQueue">Processoru izpildīšanas rinda.</param>
        public ProcessorRunner(IProcessor processor, ProcessorQueue processingQueue) {
            if (processor == null) {
                throw new ArgumentNullException("processor"); 
            }

            if (processingQueue == null) {
                throw new ArgumentNullException("processingQueue");
            }

            _processor = processor;
            _trigger = processor.Trigger;
            _processingQueue = processingQueue; 
        }

        /// <summary>
        /// Atgriež processora izpildītāja nosaukumu.
        /// </summary>
        /// <value>Processora izpildītāja nosaukums.</value>
        public string Name {
            get { return _processor.Name; }
        }

        /// <summary>
        /// Atgriež processoru.
        /// </summary>
        /// <value>Processors.</value>
        public IProcessor Processor {
            get { return _processor; }
        }

        /// <summary>
        /// Atgriež processora izpildītāja statusu.
        /// </summary>
        /// <value>Processora izpildītāja status.</value>
        public ProcessorState State {
            get { return _state; }
        }

        /// <summary>
        /// Atgriež <c>true</c>  ja processors tiek palaists, citādi atgriež <c>false</c>.
        /// </summary>
        public bool IsRunning {
            get { return _state == ProcessorState.Running; }
        }

        /// <summary>
        /// Ieveito processoru izpildīšas rindā. Ja processora izpildītājs nebija palists, tas paliž to.
        /// </summary>
        public void Force() {
            ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Force execute processor: {0}", _processor.Name);
            if (!_processingQueue.HasItemPendingOnQueue(_processor)) {
                _processingQueue.Enqueue(_processor);
            }

            this.Start();
        }

        /// <summary>
        /// Nodzēš processoru no izpildīšans rindas.
        /// </summary>
        public void CancelPending() {
            ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Cancel pending processor: {0}", _processor.Name);
            _processingQueue.RemovePendingItems(_processor);   
        }

        /// <summary>
        /// Palaiž processora izpildītāju un uzstāda processora izpildītāja statusu uz <see cref="ProcessorState.Running"/>.
        /// </summary>
        public void Start() {
            lock (this) {
                if (this.IsRunning) {
                    return;
                }

                _state = ProcessorState.Running;
            }

            // multiple thread instances cannot be created
            if (_thread == null || _thread.ThreadState == System.Threading.ThreadState.Stopped) {
                _thread = new Thread(new ThreadStart(Run));
                _thread.Name = _processor.Name;
            }

            // start thread if it's not running yet
            if (_thread.ThreadState != System.Threading.ThreadState.Running) {
                _thread.Start();
            }
        }

        /// <summary>
        /// Apstādina procesora izpildītāju un uzstāda processora izpildītāja statusu uz <see cref="ProjectIntegratorState.Stopping"/>.
        /// </summary>
        public void Stop() {
            if (this.IsRunning) {
                ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Stopping processor runner: {0}", _processor.Name);
                _state = ProcessorState.Stopping;
            }
        }

        /// <summary>
        /// Apstādinam processoru izpildītaju, nositot processora izpildītāja plūsmu.
        /// </summary>
        public void Abort() {
            if (_thread != null) {
                ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Aborting processor runner: {0}", _processor.Name);
                _thread.Abort();
            }
        }

        /// <summary>
        /// Gaidam līds processora izpildītāis tiek apstādīnāts.
        /// </summary>
        public void WaitForExit() {
            if (_thread != null && _thread.IsAlive) {
                _thread.Join();
            }
        }

        /// <summary>
        /// Ensure that the integrator's thread is aborted when this object is disposed.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            this.Abort();
        }

        /// <summary>
        /// Processora plūsmas funkcija.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't know exception type")]
        private void Run() {
            ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Starting processor runner: {0}", _processor.Name);

            try {
                // loop, until the integrator is stopped
                while (this.IsRunning) {
                    try {
                        Execute();
                    }
                    catch (Exception ex) {
                        ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Error, 0, "Processor runner {0} error: {1}", _processor.Name, ex.ToString());
                    }

                    // sleep for a short while, to avoid hammering CPU
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException) {
                // suppress logging of ThreadAbortException
                Thread.ResetAbort();
            }
            finally {
                Stopped();
            }
        }

        /// <summary>
        /// Palaiž processoru
        /// </summary>
        private void Execute() {
            if (_processingQueue.GetNextRequest(_processor)) {
                ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Processor {0} is first in queue and shall executing", _processor.Name);
                if (_trigger != null) {
                    _trigger.ProcessingCompleted();
                }

                try {
                    _processor.Start();
                }
                finally {
                    _processingQueue.Dequeue();
                }
            }
            else {
                // Pārbaudam vai triggers ir nostrāajis
                if (_trigger != null && _trigger.Fire()) {
                    if (!_processingQueue.HasItemPendingOnQueue(_processor)) {
                        ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Processor {0} trigger fired", _processor.Name);
                        _processingQueue.Enqueue(_processor);
                    }
                }

                /*
                // If a build is queued for this project we need to hang around until either:
                // - the build gets started by reaching it's turn on the queue
                // - the build gets cancelled from the queue
                // - the thread gets killed
                //while (this.IsRunning && _processingQueue.HasItemPendingOnQueue(_processor)) {
                //    Thread.Sleep(200);
                //}
                 */ 
            }
        }

        /// <summary>
        /// Processora izpildītāis tiek apstādīnāts
        /// </summary>
        private void Stopped() {
            // the state was set to 'Stopping', so set it to 'Stopped'
            _state = ProcessorState.Stopped;
            _thread = null;
            
            // Ensure that any queued integrations are cleared for this project.
            _processingQueue.RemoveItems(_processor);   

            ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Processor {0} is now stopped", _processor.Name);
        }
    }
}
