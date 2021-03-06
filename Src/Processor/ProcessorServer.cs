// ----------------------------------------------------------------------------
// <copyright file="ProcessorServer.cs" company="ABC Software Ltd">
//    Copyright © ABC SOFTWARE. All rights reserved.
//    Licensed under the Apache License, Version 2.0.  
//    See License.txt in the project root for license information. 
// </copyright>
// ----------------------------------------------------------------------------
 
namespace Abc.Processor {
    using System;
    using System.Diagnostics;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Security.Permissions;
    using System.ComponentModel;

    /// <summary>
    /// Processora serveris.
    /// </summary>
    [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure)]
    public class ProcessorServer : Component {
        private const string URI = "ProcessorManager.rem";
        private ProcessorManager _manager;
        private string _configurationFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorServer"/> class.
        /// </summary>
        public ProcessorServer() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorServer"/> class.
        /// </summary>
        /// <param name="configurationFileName">Name of the configuration file.</param>
        public ProcessorServer(string configurationFileName) {
            this._configurationFileName = configurationFileName;
        }

        /// <summary>
        /// Gets or sets the name of the configuration file.
        /// </summary>
        /// <value>
        /// The name of the configuration file.
        /// </value>
        [DefaultValue((string)null)]
        public string ConfigurationFileName {
            get { return this._configurationFileName; }
            set { this._configurationFileName = value; }
        }

        /// <summary>
        /// Gets or sets the processor manager.
        /// </summary>
        /// <value>
        /// The processor manager.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
        public ProcessorManager ProcessorManager {
            get {
                if (this._manager == null) {
                    this._manager = new ProcessorManager();
                }

                return this._manager;
            }

            set {
                this._manager = value;
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start() {
            string configurationFileName = string.IsNullOrEmpty(this._configurationFileName) 
                ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : this._configurationFileName;

            RegisterForRemoting(configurationFileName);  
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            if (this._manager != null) {
                ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Information, 0, "Disconnecting remote server");
                RemotingServices.Disconnect((MarshalByRefObject)this._manager);
            }

            foreach (IChannel channel in ChannelServices.RegisteredChannels) {
                ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Information, 0, "Unregistering channel: {0}", channel.ChannelName);
                ChannelServices.UnregisterChannel(channel);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                this.Stop(); 
            }

            base.Dispose(disposing); 
        }

        private void RegisterForRemoting(string configurationFileName) {
            RemotingConfiguration.Configure(configurationFileName, false);

            // Marshal
            MarshalByRefObject marshalByRef = (MarshalByRefObject)this.ProcessorManager;
            RemotingServices.Marshal(marshalByRef, URI, typeof(ProcessorManager));

            // Channels
            foreach (IChannel channel in ChannelServices.RegisteredChannels) {
                ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Information, 0, "Registered channel: {0}", channel.ChannelName);

                IChannelReceiver channelReciever = channel as IChannelReceiver;
                if (channelReciever != null) {
                    foreach (string url in channelReciever.GetUrlsForUri(URI)) {
                        ProcessorDiagnostic.TraceSource.TraceEvent(TraceEventType.Information, 0, "Processor: Listening on url: {0}", url);
                    }
                }
            }
        }
    }
}
