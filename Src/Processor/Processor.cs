// ----------------------------------------------------------------------------
// <copyright file="Processor.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// Processor base class.
    /// </summary>
    public class 
        Processor : ReflectorBase, IProcessor {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        protected Processor() {
            this.AutoLog = true;
        } 

        /// <summary>
        /// Gets or sets the processor name.
        /// </summary>
        /// <value>
        /// The processor name.
        /// </value>
        public string Name {
            get { return this.name ?? string.Empty; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets the processor trigger.
        /// </summary>
        /// <value>The processor trigger.</value>
        [DefaultValue((Triggers.Trigger)null)]
        public Triggers.ITrigger Trigger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to report Start, Stop, and Abort commands in the event log.
        /// </summary>
        /// <value>
        ///   <c>true</c>  to report information in the event log; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool AutoLog { get; set; }

        /// <summary>
        /// Aborts this instance.
        /// </summary>
        public void Abort() {
            try {
                this.OnAbort();
                this.WriteToLog(SR.AbortSuccessful, TraceEventType.Information);
            }
            catch (Exception exception) {
                if (ProcessorDiagnostic.IsFatal(exception)) {
                    throw;
                }

                this.WriteToLog(SR.AbortFailedFormat(exception.ToString()), TraceEventType.Error);
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start() {
            this.WriteToLog(SR.StartingProcessorFormat(this.name), TraceEventType.Information);

            try {
                this.OnStart();
                this.WriteToLog(SR.StartSuccessful, TraceEventType.Information);
            }
            catch (Exception exception) {
                if (ProcessorDiagnostic.IsFatal(exception)) {
                    throw;
                }

                this.WriteToLog(SR.StartFailedFormat(exception.ToString()), TraceEventType.Error);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            try {
                this.OnStop();
                this.WriteToLog(SR.StopSuccessful, TraceEventType.Information);
            }
            catch (Exception exception) {
                if (ProcessorDiagnostic.IsFatal(exception)) {
                    throw;
                }

                this.WriteToLog(SR.StopFailedFormat(exception.ToString()), TraceEventType.Error);
            }
        }

        /// <summary>
        /// Called when abort this instance.
        /// </summary>
        protected virtual void OnAbort() {
        }

        /// <summary>
        /// Called when start this instance.
        /// </summary>
        protected virtual void OnStart() { 
        }

        /// <summary>
        /// Called when stop this instance.
        /// </summary>
        protected virtual void OnStop() {
        }

        private void WriteToLog(string message, TraceEventType severity) {
            if (this.AutoLog) {
                ProcessorDiagnostic.TraceSource.TraceEvent(severity, 0, message);
            }
        }
    }
}
