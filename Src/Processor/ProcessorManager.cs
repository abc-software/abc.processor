// ----------------------------------------------------------------------------
// <copyright file="ProcessorManager.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Security.Permissions;

    /// <summary>
    /// Processoru vadītājs.
    /// </summary>
    public class ProcessorManager : MarshalByRefObject {
        private ProcessorRunnerCollection _processorRunners;
        private ProcessorQueue _processingQueue;
        private ProcessorCollection _processors;
        private bool _initialized;
        private object _initializeLock = new object[0]; 

        /// <summary>
        /// Inicializē jauno <see cref="ProcessorManager"/> klases instanci.
        /// </summary>
        public ProcessorManager() {
            this._processors = new ProcessorCollection(); 
        }

        /// <summary>
        /// Gets the processors.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(Design.ProcessorsCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))] 
        public ProcessorCollection Processors {
            get { return this._processors; }
        }

        /// <summary>
        /// Uzsāc visu processoru izpildīsanu.
        /// </summary>
        public void StartAllProcessors() {
            foreach (ProcessorRunner runner in this.ProcessorRunners) {
                runner.Start();  
            }
        }

        /// <summary>
        /// Uzsāc processora izpildīšanu.
        /// </summary>
        /// <param name="processorName">Processora nosaukums.</param>
        public void Start(string processorName) {
            this.GetProcessorRunner(processorName).Start(); 
        }

        /// <summary>
        /// Apstāda visu processoru izpildīšnau, gaidot visu pabeigšanu.
        /// </summary>
        public void StopAllProcessors() {
            // Apstādam visus processorus
            foreach (ProcessorRunner runner in this.ProcessorRunners) {
                runner.Stop(); 
            }
            
            // Gaidam līdz tām kad visi processori pabiegs darbu
            this.WaitForProcessorToExit();
            
            // Attīram rindu
            this._processingQueue.Clear();  
        }

        /// <summary>
        /// Apstāda processora izpildīšanu, gaidot to pabeigšanu.
        /// </summary>
        /// <param name="processorName">Processora nosaukums.</param>
        public void Stop(string processorName) {
            this.GetProcessorRunner(processorName).Stop();
        }

        /// <summary>
        /// Ievitojam processora izsaukumu izpildīšanas rindā.
        /// </summary>
        /// <param name="processorName">Processora nosaukums.</param>
        public void Force(string processorName) {
            this.GetProcessorRunner(processorName).Force();
        }

        /// <summary>
        /// Gaidam līds processora izpildīšnas pabeigšānai.
        /// </summary>
        /// <param name="processorName">Processora nosaukums.</param>
        public void WaitForExit(string processorName) {
            this.GetProcessorRunner(processorName).WaitForExit();
        }

        /// <summary>
        /// Apstāda visu processoru izpildīšanu, negaidot to pabeigšanu.
        /// </summary>
        public void AbortAllProcessors() {
            // Apstādam visus processorus neigaidot to pabeigšanu
            foreach (ProcessorRunner runner in this.ProcessorRunners) {
                runner.Abort();
            }

            // Gaidam līdz tām kad visi processori pabiegs darbu
            this.WaitForProcessorToExit();

            // Attīram rindu
            this._processingQueue.Clear();
        }

        /// <summary>
        /// Apstāda visu processoru izpildīšanu, negaidot to pabeigšanu.
        /// </summary>
        /// <param name="processorName">Processora nosaukums.</param>
        public void Abort(string processorName) {
            this.GetProcessorRunner(processorName).Abort();
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"></see> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"></see> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/></PermissionSet>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService() {
            return null;
        }

        /// <summary>
        /// Atgriežam processora izpildītāju, pēc processora nosakuma.
        /// </summary>
        /// <param name="processorName">Processora nosaukums.</param>
        /// <returns>Proecssora izpildītājs.</returns>
        private ProcessorRunner GetProcessorRunner(string processorName) {
            ProcessorRunner runner = this.ProcessorRunners[processorName];
            if (runner == null) {
                throw new InvalidOperationException(processorName); // TODO: make exception
            }

            return runner;
        }

        /// <summary>
        /// Gaidam līdz visi processori pabeidza izpildīšanu.
        /// </summary>
        private void WaitForProcessorToExit() {
            foreach (ProcessorRunner runner in this.ProcessorRunners) {
                runner.WaitForExit();
            }
        }

        private ProcessorRunnerCollection ProcessorRunners {
            get {
                if (!this._initialized) {
                    lock (this._initializeLock) {
                        Configuration.ProcessorConfigurationSection configuration =
                            Configuration.ProcessorConfiguration.ProcessorConfigurationSection;

                        ProcessorCollection processors = configuration.GetRuntimeObject();
                        foreach (var item in processors) {
                            this._processors.Add(item);
                        }

                        if (this._processors.Count == 0) {
                            ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Warning, 0, "Not configured any processors");
                        }

                        this._processingQueue = new ProcessorQueue(this._processors.Count + 1);
                        this._processorRunners = new ProcessorRunnerCollection(this._processors, this._processingQueue);

                        this._initialized = true;
                    }
                }

                return this._processorRunners;
            }
        } 
    }
}
